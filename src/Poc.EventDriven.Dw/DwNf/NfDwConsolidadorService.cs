using Azure.Storage.Blobs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.DwNf.Abstractions;
using Poc.EventDriven.DwNf.Events;
using Poc.EventDriven.DwNf.Items;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;
using Poc.EventDriven.Nfs;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Poc.EventDriven.DwNf;

public class NfDwConsolidadorService : IMessageBusBatchEventHandler<NfConsolidacaoEvent>
{
    private static byte BUFFER_SIZE = 8;

    private readonly ILogger<NfDwConsolidadorService> _logger;
    private readonly BlobContainerClient _container;
    private readonly IRelationalDbContextFactory<IDwNfDbContext> _contextFactory;

    public NfDwConsolidadorService(
        ILogger<NfDwConsolidadorService> logger,
        IAzureClientFactory<BlobServiceClient> blobClientFactory,
        IRelationalDbContextFactory<IDwNfDbContext> dbContext)
    {
        _logger = logger;
        _contextFactory = dbContext;
        _container = blobClientFactory
            .CreateClient(NfStorageConsts.NfStorageName)
            .GetBlobContainerClient(NfStorageConsts.NfContainerName);
    }

    public async Task HandleBatchAsync(List<MessageWithBag<NfConsolidacaoEvent>> batch, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando consolidação");

        var nfDocuments = new List<NfDocument>(128); // Tamanho máximo matematicamente possível
        var uniqueCnpjs = new HashSet<string>(3 * 128);
        var uniqueDates = new HashSet<DateTime>(128);
        var uniqueSkus = new HashSet<string>(); // Não há previsibilidade de tamanho máximo 😢
        var blobAddresses = new Dictionary<Guid, string>();

        _logger.LogTrace($"Baixando notas fiscais");
        await foreach (var (nfDocument, blobAddress) in DownloadNfAsync(batch.Select(x => x.Body), cancellationToken))
        {
            nfDocuments.Add(nfDocument);
            blobAddresses[nfDocument.Chave] = blobAddress;
            
            if (!string.IsNullOrWhiteSpace(nfDocument.Empresa?.Cnpj))
                uniqueCnpjs.Add(nfDocument.Empresa.Cnpj);

            if (!string.IsNullOrWhiteSpace(nfDocument.Emissor?.Cnpj))
                uniqueCnpjs.Add(nfDocument.Emissor.Cnpj);

            if (!string.IsNullOrWhiteSpace(nfDocument.Exportador?.Cnpj))
                uniqueCnpjs.Add(nfDocument.Exportador.Cnpj);

            uniqueDates.Add(nfDocument.Emissao.Date);
            uniqueSkus.UnionWith(nfDocument.Items.Select(q => q.Sku).Distinct());
        }

        _logger.LogTrace("Searialização de documentos concluída.");

        try
        {
            var tiposOperacoes = await ConsolidarDimTipoOperacaoAsync(cancellationToken);
            var empresas = await ConsolidarDimEmpresasAsync(uniqueCnpjs, nfDocuments, cancellationToken);
            var datas = await ConsolidarDimTempoAsync(uniqueDates, nfDocuments, cancellationToken);
            var skus = await ConsolidarDimSkusAsync(uniqueSkus, nfDocuments, cancellationToken);
            var dimNfs = await ConsolidarDimNfsAsync(nfDocuments, blobAddresses, cancellationToken);

            _logger.LogTrace("Consolidação de dimenções concluída");

            await ConsolidarFactNfsAsync(nfDocuments, blobAddresses, tiposOperacoes, empresas, datas, cancellationToken);
            await ConsolidarFactNfItemAsync(nfDocuments, tiposOperacoes, empresas, datas, skus, dimNfs, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            // Rescheduling
            foreach (var bag in batch.Select(m => m.Bag))
            {
                if (bag.Attempts > 10)
                {
                    await bag.AbortAsync(ex);
                }
                else
                {
                    await bag.ReScheduleAsync();
                }
            }
        }
        
        _logger.LogInformation("Consolidação concluída.");
    }

    private async Task ConsolidarFactNfItemAsync(
        List<NfDocument> nfDocuments,
        Dictionary<TipoOperacao, int> tiposOperacoes,
        Dictionary<string, int> empresas,
        Dictionary<DateTime, int> datas,
        Dictionary<string, int> skus,
        Dictionary<Guid, int> nfs,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await _contextFactory.CreateDbContextAsync();
        // NfItem é um caso especial... não vamos gastar recursos tentando atualizar as linhas... é mais eficiente dropar e inserir denovo...
        context.FactNfItems
            .RemoveRange(
                context.FactNfItems
                    .Include(nfi => nfi.DimNf)
                    .Where(nfi => nfDocuments
                        .Select(nf => nf.Chave)
                        .Contains(nfi.DimNf.Chave)));

        await context.SaveChangesAsync(cancellationToken);

        context.FactNfItems
            .AddRange(
                nfDocuments
                    .SelectMany(nf => nf.Items.Select(nfItem => new { Nf = nf, NfItem = nfItem }))
                    .Select(x =>
                    {
                        var nfItem = new FactNfItem();

                        nfItem.DimNfId = nfs[x.Nf.Chave];
                        nfItem.DimSkuId = skus[x.NfItem.Sku];

                        if (x.Nf.Empresa?.Cnpj != null)
                            nfItem.DimEmpresaId = empresas[x.Nf.Empresa.Cnpj];
                        
                        if (x.Nf.Exportador?.Cnpj != null)
                            nfItem.DimExportadorId = empresas[x.Nf.Exportador.Cnpj];
                        
                        if (x.Nf.Emissor?.Cnpj != null)
                            nfItem.DimEmissorId = empresas[x.Nf.Emissor.Cnpj];
                        
                        nfItem.DimEmissaoId = datas[x.Nf.Emissao.Date];
                        nfItem.DimTipoOperacaoId = tiposOperacoes[x.Nf.Operacao];
                        
                        nfItem.Descricao = x.NfItem.Descricao;
                        nfItem.Quantidade = x.NfItem.Quantidade;
                        nfItem.ValorBrl = x.NfItem.ValorBrl;
                        nfItem.ValorIcmsBrl = x.NfItem.ValorIcmsBrl;
                        nfItem.ValorIcmsstBrl = x.NfItem.ValorIcmsstBrl;
                        nfItem.ValorIiBrl = x.NfItem.ValorIiBrl;
                        nfItem.ValorIpiBrl = x.NfItem.ValorIpiBrl;
                        nfItem.ValorPisBrl = x.NfItem.ValorPisBrl;
                        nfItem.ValorCofinsBrl = x.NfItem.ValorCofinsBrl;

                        return nfItem;
                    }));

        await context.SaveChangesAsync(cancellationToken);
        _logger.LogTrace("FactNfItem consolidados com sucesso!");
    }

    private async Task ConsolidarFactNfsAsync(
        List<NfDocument> nfs,
        Dictionary<Guid, string> addresses,
        Dictionary<TipoOperacao, int> tiposOperacoes,
        Dictionary<string, int> empresas,
        Dictionary<DateTime, int> datas,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var context = await _contextFactory.CreateDbContextAsync();
        var nfsAAtualizar = await context.FactNfs.Where(q => nfs.Select(nf => nf.Chave).Contains(q.Chave)).ToListAsync();

        if (nfsAAtualizar.Any())
        {
            var nfDict = nfs.ToDictionary(k => k.Chave);
            context.FactNfs.UpdateRange(nfsAAtualizar
                .Select(nf =>
                {
                    nf.DimEmpresaId = nfDict[nf.Chave]?.Empresa?.Cnpj != null
                        ? empresas[nfDict[nf.Chave].Empresa!.Cnpj]
                        : 0;

                    nf.DimExportadorId = nfDict[nf.Chave]?.Exportador?.Cnpj != null
                        ? empresas[nfDict[nf.Chave].Exportador!.Cnpj]
                        : null;

                    nf.DimEmissorId = nfDict[nf.Chave]?.Emissor?.Cnpj != null
                        ? empresas[nfDict[nf.Chave].Emissor!.Cnpj]
                        : 0;

                    nf.DimEmissaoId = nfDict[nf.Chave]?.Emissao != null
                        ? datas[nfDict[nf.Chave].Emissao.Date]
                        : 0;

                    nf.DimOperacaoId = tiposOperacoes[nfDict[nf.Chave].Operacao];

                    nf.Numero = nfDict[nf.Chave].Numero;
                    nf.Serie = nfDict[nf.Chave].Serie;
                    nf.BlobAddress = addresses[nf.Chave];
                    nf.ValorTotalBrl = nfDict[nf.Chave].Items.Sum(i => i.ValorBrl);
                    nf.ValorIcmsBrl = nfDict[nf.Chave].Items.Sum(i => i.ValorIcmsBrl);
                    nf.ValorIcmsstBrl = nfDict[nf.Chave].Items.Sum(i => i.ValorIcmsstBrl);
                    nf.ValorIiBrl = nfDict[nf.Chave].Items.Sum(i => i.ValorIiBrl);
                    nf.ValorIpiBrl = nfDict[nf.Chave].Items.Sum(i => i.ValorIpiBrl);
                    nf.ValorPisBrl = nfDict[nf.Chave].Items.Sum(i => i.ValorPisBrl);
                    nf.ValorCofinsBrl = nfDict[nf.Chave].Items.Sum(i => i.ValorCofinsBrl);

                    return nf;
                }));

            _logger.LogTrace("Notas fiscais atualizadas");
        }

        var nfsAInserir = nfs
            .ExceptBy(nfsAAtualizar.Select(nf => nf.Chave), nf => nf.Chave)
            .Select(nf =>
            {
                var fact = new FactNf();

                fact.DimEmpresaId = nf?.Empresa?.Cnpj != null
                        ? empresas[nf.Empresa!.Cnpj]
                        : 0;

                fact.DimExportadorId = nf?.Exportador?.Cnpj != null
                    ? empresas[nf.Exportador!.Cnpj]
                    : null;

                fact.DimEmissorId = nf?.Emissor?.Cnpj != null
                    ? empresas[nf.Emissor!.Cnpj]
                    : 0;

                fact.DimEmissaoId = nf?.Emissao != null
                    ? datas[nf.Emissao.Date]
                    : 0;

                fact.DimOperacaoId = tiposOperacoes[nf!.Operacao];

                fact.Chave = nf.Chave;
                fact.Numero = nf.Numero;
                fact.Serie = nf.Serie;
                fact.BlobAddress = addresses[nf.Chave];
                fact.ValorTotalBrl = nf.Items.Sum(i => i.ValorBrl);
                fact.ValorIcmsBrl = nf.Items.Sum(i => i.ValorIcmsBrl);
                fact.ValorIcmsstBrl = nf.Items.Sum(i => i.ValorIcmsstBrl);
                fact.ValorIiBrl = nf.Items.Sum(i => i.ValorIiBrl);
                fact.ValorIpiBrl = nf.Items.Sum(i => i.ValorIpiBrl);
                fact.ValorPisBrl = nf.Items.Sum(i => i.ValorPisBrl);
                fact.ValorCofinsBrl = nf.Items.Sum(i => i.ValorCofinsBrl);

                return fact;
            });

        if (nfsAInserir.Any())
        {
            context.FactNfs.AddRange(nfsAInserir);
            _logger.LogTrace("Novas notas fiscais inseridas.");
        }

        await context.SaveChangesAsync(cancellationToken);
        _logger.LogTrace("Notas fiscais consolidadas");
    }

    private async Task<Dictionary<Guid, int>> ConsolidarDimNfsAsync(List<NfDocument> nfs, Dictionary<Guid, string> addresses, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var attempts = 0;
        List<DimNf>? notasFiscais = null;

        do
        {
            try
            {
                var context = await _contextFactory.CreateDbContextAsync();
                notasFiscais = await context.DimNfs.Where(q => nfs.Select(nf => nf.Chave).Contains(q.Chave)).ToListAsync();
                var notasToAdd = nfs
                    .Select(nf => ParseDimNf(nf, addresses[nf.Chave]))
                    .ExceptBy(notasFiscais.Select(nf => nf.Chave), nf => nf.Chave)
                    .ToList();  // We need to materialize so EF can track id back...;

                if (notasToAdd.Any())
                {
                    context.DimNfs.AddRange(notasToAdd);
                    await context.SaveChangesAsync(cancellationToken);
                    notasFiscais = notasFiscais.Union(notasToAdd).ToList();
                }
                attempts = 0;
            }
            catch
            {
                _logger.LogWarning("Falha de integridade ao consolidar DimNfs, tentando novamente...");
                if (attempts == 1) throw;
            }
            finally
            {
                attempts--;
            }
        } while (attempts > 0);

        _logger.LogTrace("DimNfs consolidadas com sucesso!");

        return notasFiscais?.ToDictionary(k => k.Chave, p => p.Id) ?? new Dictionary<Guid, int>();
    }

    private async Task<Dictionary<string, int>> ConsolidarDimSkusAsync(HashSet<string> uniqueSkus, List<NfDocument> nfs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var attempts = 3;
        List<DimSku>? skus = null;

        do
        {
            try
            {
                var context = await _contextFactory.CreateDbContextAsync();
                skus = await context.DimSkus.Where(q => uniqueSkus.Contains(q.Sku)).ToListAsync();
                var skusToAdd = nfs
                    .SelectMany(nf => nf.Items.Select(item => item.Sku))
                    .Distinct()
                    .Except(skus.Select(sku => sku.Sku))
                    .Select(sku => new DimSku { Sku = sku })
                    .ToList();  // We need to materialize so EF can track id back...;

                if (skusToAdd.Any())
                {
                    context.DimSkus.AddRange(skusToAdd);
                    await context.SaveChangesAsync(cancellationToken);
                    skus = skus.Union(skusToAdd).ToList();
                }
                attempts = 0;
            }
            catch
            {
                _logger.LogWarning("Falha de integridade ao consolidar DimSkus, tentando novamente...");
                if (attempts == 1) throw;
            }
            finally
            {
                attempts--;
            }
        } while (attempts > 0);

        _logger.LogTrace("DimSku consolidado com sucesso!");

        return skus?.ToDictionary(q => q.Sku, p => p.Id) ?? new Dictionary<string, int>();
    }

    private async Task<Dictionary<DateTime, int>> ConsolidarDimTempoAsync(HashSet<DateTime> uniqueDates, List<NfDocument> nfs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var attempts = 3;
        List<DimTempo>? datas = null;

        do
        {
            try
            {
                var context = await _contextFactory.CreateDbContextAsync();
                datas = await context.DimTempo.Where(q => uniqueDates.Contains(q.Date)).ToListAsync();
                var dimTempoToAdd = nfs
                    .Select(nf => nf.Emissao.Date)
                    .Distinct()
                    .Except(datas.Select(q => q.Date))
                    .Select(data => ParseDimTempo(data))
                    .ToList();  // We need to materialize so EF can track id back...;

                if (dimTempoToAdd.Any())
                {
                    context.DimTempo.AddRange(dimTempoToAdd);
                    await context.SaveChangesAsync(cancellationToken);
                    datas = datas.Union(dimTempoToAdd).ToList();
                }
                attempts = 0;
            }
            catch
            {
                _logger.LogWarning("Falha de integridade ao consolidar DimTempo, tentando novamente...");
                if (attempts == 1) throw;
            }
            finally
            {
                attempts--;
            }
        } while(attempts > 0);

        _logger.LogTrace("DimTempo consolidado com sucesso!");

        return datas?.ToDictionary(q => q.Date, p => p.Id) ?? new Dictionary<DateTime, int>();
    }

    private async Task<Dictionary<string, int>> ConsolidarDimEmpresasAsync(HashSet<string> uniqueCnpjs, List<NfDocument> nfs, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var attempts = 3;
        IEnumerable<DimEmpresa>? empresas = null;

        do
        {
            try
            {
                var context = await _contextFactory.CreateDbContextAsync();
                empresas = await context.DimEmpresas.Where(q => uniqueCnpjs.Contains(q.Cnpj)).ToListAsync();
                var empresasToAdd = nfs
                    .SelectMany(nf =>
                    {
                        var list = new List<EmpresaDocument>();
                        if (!string.IsNullOrEmpty(nf.Empresa?.Cnpj))
                            list.Add(nf.Empresa);
                        if (!string.IsNullOrEmpty(nf.Emissor?.Cnpj))
                            list.Add(nf.Emissor);
                        if (!string.IsNullOrEmpty(nf.Exportador?.Cnpj))
                            list.Add(nf.Exportador);
                        return list;
                    })
                    .DistinctBy(q => q.Cnpj)
                    .ExceptBy(empresas.Select(q => q.Cnpj), q => q.Cnpj)
                    .Select(empresa => ParseDimEmpresa(empresa))
                    .ToList();  // We need to materialize so EF can track id back...

                if (empresasToAdd.Any())
                {
                    context.DimEmpresas.AddRange(empresasToAdd);
                    await context.SaveChangesAsync(cancellationToken);
                    empresas = empresas.Union(empresasToAdd).ToList();
                }
                attempts = 0;
            }
            catch
            {
                _logger.LogWarning("Falha de integridade ao consolidar DimEmpresa, tentando novamente...");
                if (attempts == 1) throw;
            }
            finally
            {
                attempts--;
            }
        } while (attempts > 0);

        _logger.LogTrace("DimEmpresas consolidadas");

        return empresas?.ToDictionary(q => q.Cnpj, p => p.Id) ?? new Dictionary<string, int>();
    }

    private async Task<Dictionary<TipoOperacao, int>> ConsolidarDimTipoOperacaoAsync(CancellationToken cancellationToken)
    {
        var operacoes = new TipoOperacao[] { TipoOperacao.Saida, TipoOperacao.Entrada, TipoOperacao.NaoAtribuido };
        var attempt = 3;
        List<DimTipoOperacao>? consultaTipoOperacoes = null;

        do
        {
            try
            {
                var context = await _contextFactory.CreateDbContextAsync();
                consultaTipoOperacoes = await context.DimTipoOperacoes.ToListAsync(cancellationToken);
                if (consultaTipoOperacoes.Count() == 0)
                {
                    var novosTiposOperacoes = operacoes
                        .Select(operacao => new DimTipoOperacao
                        {
                            TipoOperacao = operacao,
                            Descricao = operacao switch
                            {
                                TipoOperacao.Entrada => "Entrada",
                                TipoOperacao.Saida => "Saída",
                                _ => "Não Atribuído"
                            }
                        })
                        .ToList(); // We need to materialize so EF can track id back...

                    context.DimTipoOperacoes.AddRange(novosTiposOperacoes);
                    await context.SaveChangesAsync(cancellationToken);
                    consultaTipoOperacoes.AddRange(novosTiposOperacoes);
                }
                attempt = 0;
            }
            catch
            {
                _logger.LogWarning($"Falha de integridade ao tentar consolidar DimTipoOperação. Tentando novamente...");
                if (attempt == 1) throw;
            }
            finally
            {
                attempt--;
            }
        } while (attempt > 0);

        return consultaTipoOperacoes?.ToDictionary(k => k.TipoOperacao, p => p.Id) ?? new Dictionary<TipoOperacao, int>();
    }

    private DimTempo ParseDimTempo(DateTime tempo)
        => new DimTempo
        {
            Ano = tempo.Year,
            Mes = tempo.Month,
            Dia = tempo.Day,
            Date = tempo.Date
        };

    private DimEmpresa ParseDimEmpresa(EmpresaDocument empresa)
        => new DimEmpresa
        {
            Cnpj = empresa.Cnpj,
            Nome = empresa.Nome
        };

    private DimNf ParseDimNf(NfDocument nf, string blobAddress)
        => new DimNf
        {
            Chave = nf.Chave,
            Numero = nf.Numero,
            Serie = nf.Serie,
            BlobAddress = blobAddress
        };

    private async IAsyncEnumerable<(NfDocument, string)> DownloadNfAsync(
        IEnumerable<NfConsolidacaoEvent> batch,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var chunk in batch.Chunk(BUFFER_SIZE))
        {
            if (cancellationToken.IsCancellationRequested) break;

            _logger.LogTrace($"Fetching blob batch {BUFFER_SIZE}");
            
            var buffer = chunk.Select(x => _container.GetBlobClient(x.BlobAddress).DownloadContentAsync()).ToList();
            await Task.WhenAll(buffer);

            for (var index = 0; index < buffer.Count(); index++)
            {
                var nfDocument = JsonSerializer.Deserialize<NfDocument>(buffer[index].Result.Value.Content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (nfDocument == null) throw new InvalidOperationException($"Não foi possível desserializar a NF {chunk[index].ChaveNf}");

                yield return (nfDocument!, chunk[index].BlobAddress);
            }
        }
    }
}
