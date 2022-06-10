using Bogus;
using Bogus.Extensions.Brazil;

using Google.Protobuf.WellKnownTypes;

using Grpc.Net.Client;

using Poc.EventDriven.Boms;
using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Clientes;
using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Produtos.Abstractions;
using Poc.EventDriven.Protos.NfIngress.V1;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Nfs;

internal static class NfsCmdHandler
{
    public static async Task Gerar(string regimesUri, string grpcUri, string cnpj, int quantidade, string periodo, bool apenasImport, bool apenasExport, bool apenasNac, string[] fornecedoresCnpj, string[] clientesCnpj)
    {
        Console.WriteLine($"Gerando {quantidade} NFs para {cnpj} em {periodo} - i: {apenasImport} e: {apenasExport} n: {apenasNac}.");
        Console.WriteLine($"Regimes: {regimesUri}");
        Console.WriteLine($"Ingestor: {grpcUri}");

        var clienteClient = new ClienteApiClient(regimesUri);
        var empresaClient = new EmpresaApiClient(regimesUri);
        var bomClient = new BomApiClient(regimesUri);
        var produtoClient = new ProdutoApiClient(regimesUri);
        var faker = new Faker();
        var periodoArray = periodo.Split("/").Select(x => int.Parse(x)).ToArray();
        var periodoStart = new DateTime(periodoArray[1], periodoArray[0], 1);
        var periodoEnd = periodoStart.AddMonths(1).AddDays(-1);

        var cliente = await PersistirClienteAsync(clienteClient, empresaClient, faker, cnpj);
        var boms = await bomClient.ListarAsync(new SearchBomRequest { ByEmpresa = cliente.Id });

        if (boms.Count == 0)
        {
            Console.WriteLine($"Abortando geração! Não foram encontradas BOMs para o cnpj {cnpj}. Por favor gere boms primeiro.");
        }

        var produtos = await produtoClient.ListarProdutosEmBomAsync(boms.Select(bom => bom.Id).ToArray());

        var operacoesPermitidas = new List<string>();
        List<EmpresaDto> fornecedores = new();
        List<EmpresaDto> clientes = new();
        List<EmpresaDto> exportadores = new();
        
        if (apenasImport || !apenasImport && !apenasExport && !apenasNac)
        {
            operacoesPermitidas.Add("importacao");
            fornecedores = await PersistirEmpresasAsync(empresaClient, faker, fornecedoresCnpj);
        }
        
        if (apenasNac || !apenasImport && !apenasExport && !apenasNac)
        {
            operacoesPermitidas.Add("nacionalizacao");
            clientes = await PersistirEmpresasAsync(empresaClient, faker, clientesCnpj);
        }
        
        if (apenasExport || !apenasImport && !apenasExport && !apenasNac)
        {
            operacoesPermitidas.Add("exportacao");
            exportadores = await PersistirEmpresasAsync(empresaClient, faker, Array.Empty<string>());
        }

        AppContext.SetSwitch(
            "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        var handler = new SocketsHttpHandler
        {
            //PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(120),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };
        var channel = GrpcChannel.ForAddress(grpcUri, new GrpcChannelOptions
        {
            HttpHandler = handler
        });
        var nfClient = new NfIngressService.NfIngressServiceClient(channel);
        var sender = nfClient.AddMany();

        for (var x = 0; x < quantidade; x++)
        {
            var nf = (faker.Random.ListItem(operacoesPermitidas)) switch
            {
                "importacao"     => GerarNfImportacao(ref faker, ref periodoStart, ref periodoEnd, ref cliente, ref fornecedores, ref produtos),
                "nacionalizacao" => GerarNfNacionalizacao(ref faker, ref periodoStart, ref periodoEnd, ref cliente, ref clientes, ref boms),
                "exportacao"     => GerarNfExportacao(ref faker, ref periodoStart, ref periodoEnd, ref cliente, ref clientes, ref exportadores, ref boms),
                _ => throw new NotImplementedException()
            };
            await sender.RequestStream.WriteAsync(new NfIngressAddManyRequest
            {
                Body = nf
            });

            Console.WriteLine($"Nf {nf.Chave} gerada");
        }

        await sender.RequestStream.CompleteAsync();
        var response = await sender.ResponseAsync;

        Console.WriteLine($"{response.StatusCode.ToString()} {response.Body}");
    }

    private static NfMessage GerarNfExportacao(ref Faker faker, ref DateTime periodoStart, ref DateTime periodoEnd, ref ClienteDto emissor, ref List<EmpresaDto> clientes, ref List<EmpresaDto> exportadores, ref List<BomDto> boms)
    {
        var empresa = faker.Random.ListItem(clientes);
        var exportador = faker.Random.ListItem(clientes);
        var emissao = faker.Date.Between(periodoStart, periodoEnd);
        var nf = new NfMessage
        {
            Chave = Guid.NewGuid().ToString(),
            Emissao = Timestamp.FromDateTime(DateTime.SpecifyKind(emissao, DateTimeKind.Utc)),
            Emissor = new NfEmpresaMessage
            {
                Cnpj = emissor.Empresa.Cnpj,
                Nome = emissor.Empresa.Nome,
                RazaoSocial = $"{emissor.Empresa.Nome} Ltda"
            },
            Empresa = new NfEmpresaMessage
            {
                Cnpj = empresa.Cnpj,
                Nome = empresa.Nome,
                RazaoSocial = $"{empresa.Nome} Ltda"
            },
            Exportador = new NfEmpresaMessage
            {
                Cnpj = exportador.Cnpj,
                Nome = exportador.Nome,
                RazaoSocial = $"{exportador.Nome} Ltda"
            },
            Numero = int.Parse($"{emissao.Year}{emissao.Month}{faker.Random.Number(1, 999).ToString().PadLeft(3, '0')}"),
            Operacao = NfTipoOperacao.Saida,
            Serie = 1
        };

        foreach (var item in faker.Random.ListItems(boms.Select(bom => bom.Produto).ToList(), boms.Count() > 10 ? 10 : boms.Count()))
        {
            nf.Items.Add(GenerateNfItem(ref faker, item));
        }

        return nf;
    }

    private static NfMessage GerarNfNacionalizacao(ref Faker faker, ref DateTime periodoStart, ref DateTime periodoEnd, ref ClienteDto emissor, ref List<EmpresaDto> clientes, ref List<BomDto> boms)
    {
        var empresa = faker.Random.ListItem(clientes);
        var emissao = faker.Date.Between(periodoStart, periodoEnd);
        var nf = new NfMessage
        {
            Chave = Guid.NewGuid().ToString(),
            Emissao = Timestamp.FromDateTime(DateTime.SpecifyKind(emissao, DateTimeKind.Utc)),
            Emissor = new NfEmpresaMessage
            {
                Cnpj = emissor.Empresa.Cnpj,
                Nome = emissor.Empresa.Nome,
                RazaoSocial = $"{emissor.Empresa.Nome} Ltda"
            },
            Empresa = new NfEmpresaMessage
            {
                Cnpj = empresa.Cnpj,
                Nome = empresa.Nome,
                RazaoSocial = $"{empresa.Nome} Ltda"
            },
            Numero = int.Parse($"{emissao.Year}{emissao.Month}{faker.Random.Number(1, 999).ToString().PadLeft(3, '0')}"),
            Operacao = NfTipoOperacao.Saida,
            Serie = 1
        };

        foreach (var item in faker.Random.ListItems(boms.Select(bom => bom.Produto).ToList(), boms.Count() > 10 ? 10 : boms.Count()))
        {
            nf.Items.Add(GenerateNfItem(ref faker, item));
        }

        return nf;
    }

    private static NfMessage GerarNfImportacao(ref Faker faker, ref DateTime periodoStart, ref DateTime periodoEnd, ref ClienteDto cliente, ref List<EmpresaDto> fornecedores, ref List<ProdutoDto> produtos)
    {
        var fornecedor = faker.Random.ListItem(fornecedores);
        var emissao = faker.Date.Between(periodoStart, periodoEnd);
        var nf = new NfMessage
        {
            Chave = Guid.NewGuid().ToString(),
            Emissao = Timestamp.FromDateTime(DateTime.SpecifyKind(emissao, DateTimeKind.Utc)),
            Emissor = new NfEmpresaMessage
            {
                Cnpj = fornecedor.Cnpj,
                Nome = fornecedor.Nome,
                RazaoSocial = $"{fornecedor.Nome} Ltda"
            },
            Empresa = new NfEmpresaMessage
            {
                Cnpj = cliente.Empresa.Cnpj,
                Nome = cliente.Empresa.Nome,
                RazaoSocial = $"{cliente.Empresa.Nome} Ltda"
            },
            Numero = int.Parse($"{emissao.Year}{emissao.Month}{faker.Random.Number(1, 999).ToString().PadLeft(3, '0')}"),
            Operacao = NfTipoOperacao.Entrada,
            Serie = 1
        };

        foreach (var item in faker.Random.ListItems(produtos, produtos.Count() > 10 ? 10 : produtos.Count()))
        {
            nf.Items.Add(GenerateNfItem(ref faker, item));
        }

        return nf;
    }

    private static NfItemMessage GenerateNfItem(ref Faker faker, ProdutoDto produto)
    {
        var nfItem = new NfItemMessage
        {
            Descricao = produto.Descricao,
            Quantidade = faker.Random.Number(1, 1000),
            Sku = produto.Sku,
            ValorCofinsBrl = faker.Random.Number(1000, 2000),
            ValorIcmsBrl = faker.Random.Number(1000, 3000),
            ValorIcmsstBrl = 0,
            ValorIiBrl = faker.Random.Number(100, 1000),
            ValorIpiBrl = 0
        };

        nfItem.ValorBrl = (int)((nfItem.ValorCofinsBrl + nfItem.ValorIcmsBrl + nfItem.ValorIiBrl) / 0.57);

        return nfItem;
    }

    private static async Task<ClienteDto> PersistirClienteAsync(ClienteApiClient clientesApi, EmpresaApiClient empresasApi, Faker faker, string cnpj)
    {
        var clientes = await clientesApi.ListarAsync(new SearchClienteRequest { ByCnpj = cnpj });
        if (clientes.Any()) return clientes[0];

        var empresa = await PersistirEmpresaAsync(empresasApi, faker, cnpj);
        var cliente = await clientesApi.CriarAsync(new CreateUpdateClienteDto
        {
            EmpresaId = empresa.Id,
            Name = empresa.Nome
        });
        cliente.Empresa = empresa;
        return cliente;
    }

    private static async Task<List<EmpresaDto>> PersistirEmpresasAsync(EmpresaApiClient empresaClient, Faker faker, string[] empresasCnpjs)
    {
        if (empresasCnpjs.Length > 0)
        {
            return (await Task.WhenAll(empresasCnpjs.Select(cnpj => PersistirEmpresaAsync(empresaClient, faker, cnpj)))).ToList();
        }

        var requests = Enumerable
                .Range(0, faker.Random.Number(10, 20))
                .Select(x => PersistirEmpresaAsync(empresaClient, faker));

        return (await Task.WhenAll(requests)).ToList();
    }

    private static async Task<EmpresaDto> PersistirEmpresaAsync(EmpresaApiClient empresasApi, Faker faker, string? cnpj = null)
    {
        if (cnpj != null)
        {
            var empresas = await empresasApi.ListarAsync(new SearchEmpresaRequest { ByCnpj = cnpj });
            if (empresas.Any()) return empresas[0];
        }

        return await empresasApi.CriarAsync(new CreateUpdateEmpresaDto
        {
            Cnpj = cnpj ?? faker.Company.Cnpj(),
            Nome = faker.Company.CompanyName()
        });
    }
}
