using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Grpc.Core;

using Microsoft.Extensions.Azure;

using Poc.EventDriven.DwNf.Events;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.Nfs;
using Poc.EventDriven.Nfs.Items;
using Poc.EventDriven.Protos.NfIngress.V1;

using System.Text;
using System.Text.Json;

namespace Poc.EventDriven.Services;

public class NfIngressService : Protos.NfIngress.V1.NfIngressService.NfIngressServiceBase
{
    private readonly IMessageBusDispatcher _messageBusDispatcher;
    private readonly ILogger<NfIngressService> _logger;
    private readonly IAzureClientFactory<BlobServiceClient> _azureBlobClientFactory;

    public NfIngressService(
        IMessageBusDispatcher messageBusDispatcher,
        ILogger<NfIngressService> logger,
        IAzureClientFactory<BlobServiceClient> azureBlobClientFactory)
    {
        _messageBusDispatcher = messageBusDispatcher;
        _logger = logger;
        _azureBlobClientFactory = azureBlobClientFactory;
    }

    public async override Task<NfIngressAddManyResponse> AddMany(IAsyncStreamReader<NfIngressAddManyRequest> requestStream, ServerCallContext context)
    {
        _logger.LogTrace("Recebido lote de notas fiscais.");

        try
        {
            var container = _azureBlobClientFactory
                .CreateClient("NotasFiscaisStorage")
                .GetBlobContainerClient("notas-fiscais");

            await foreach (var request in requestStream.ReadAllAsync())
            {
                var nf = ParseNfMessage(request.Body);
                var nfJsonString = JsonSerializer.Serialize(nf, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var blobClient = container.GetBlobClient($"{nf.Chave}.json");
                await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(nfJsonString)), new BlobHttpHeaders
                {
                    ContentType = "application/json",
                    ContentEncoding = "UTF-8"
                });

                var cliente = (nf.Operacao) switch
                {
                    (TipoOperacao.Saida) => nf.Emissor!.Cnpj,
                    (_)                  => nf.Empresa!.Cnpj,
                };
                var ingressType = (nf.Operacao, nf.Exportador?.Cnpj != null) switch
                {
                    (TipoOperacao.Saida, true)  => "exportacao",
                    (TipoOperacao.Saida, false) => "nacionalizacao",
                    (_, _)                      => "importacao",
                };

                await _messageBusDispatcher.DispatchAsync(new NfConsolidacaoEvent
                {
                    BloblAddress = $"{nf.Chave}.json",
                    ChaveNf = nf.Chave.ToString(),
                    Cliente = cliente
                }, new Dictionary<string, object>()
                {
                    { "Ingress-Type", ingressType } 
                }, sessionId: cliente);
            }

            return new NfIngressAddManyResponse()
            {
                StatusCode = 200,
                Body = "Notas recebidas com sucesso!"
            };
        }
        catch (Exception ex)
        {
            return new NfIngressAddManyResponse()
            {
                StatusCode = 500,
                Body = ex.ToString()
            };
        }
        finally
        {
            _logger.LogInformation("Notas fiscais encaminhadas com sucesso!");
        }
    }

    private NfDocument ParseNfMessage(NfMessage nfMessage)
        => new NfDocument
        {
            Chave = Guid.Parse(nfMessage.Chave),
            Emissao = nfMessage.Emissao.ToDateTime(),
            Emissor = (string.IsNullOrEmpty(nfMessage.Emissor?.Cnpj) == false)
                ? ParseEmpresaMessage(nfMessage.Emissor)
                : null,
            Empresa = (string.IsNullOrEmpty(nfMessage.Empresa?.Cnpj) == false)
                ? ParseEmpresaMessage(nfMessage.Empresa)
                : null,
            Exportador = (string.IsNullOrEmpty(nfMessage.Exportador?.Cnpj) == false)
                ? ParseEmpresaMessage(nfMessage.Exportador)
                : null,
            Numero = nfMessage.Numero,
            Operacao = nfMessage.Operacao switch
            {
                NfTipoOperacao.Entrada => TipoOperacao.Entrada,
                NfTipoOperacao.Saida => TipoOperacao.Saida,
                _ => TipoOperacao.NaoAtribuido
            },
            Serie = nfMessage.Serie,
            Items = nfMessage.Items.Select(item => ParseItem(item)).ToList()
        };

    private EmpresaDocument ParseEmpresaMessage(NfEmpresaMessage empresa)
        => new EmpresaDocument
        {
            Cnpj = empresa.Cnpj,
            Nome = empresa.Nome
        };

    private NfItemDocument ParseItem(NfItemMessage item)
        => new NfItemDocument
        {
            Descricao = item.Descricao,
            Sku = item.Sku,
            Quantidade = item.Quantidade,
            ValorBrl = item.ValorBrl,
            ValorCofinsBrl = item.ValorCofinsBrl,
            ValorIcmsBrl = item.ValorIcmsBrl,
            ValorIcmsstBrl = item.ValorIcmsstBrl,
            ValorIiBrl = item.ValorIiBrl,
            ValorIpiBrl = item.ValorIpiBrl,
            ValorPisBrl = item.ValorPisBrl
        };
}
