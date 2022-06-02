using Bogus;
using Bogus.Extensions.Brazil;

using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Boms.Items.Abstractions;
using Poc.EventDriven.Clientes;
using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Produtos.Abstractions;

namespace Poc.EventDriven.Boms;

internal static class BomsCmdHandler
{
    public static async Task Gerar(string serverUri, string cnpj, int quantidade, string[] produtosSku)
    {
        Console.WriteLine($"Gerando {quantidade} BOMs para {cnpj} em {serverUri}");

        var clienteClient = new ClienteApiClient(serverUri);
        var empresaClient = new EmpresaApiClient(serverUri);
        var produtoClient = new ProdutoApiClient(serverUri);
        var bomClient = new BomApiClient(serverUri);
        var faker = new Faker();

        var cliente = await PersistirClienteAsync(clienteClient, empresaClient, faker, cnpj);
        var bomCount = await bomClient.CountBomByClientId(cliente.Id);

        if (bomCount > 0)
        {
            Console.WriteLine($"Já existem {bomCount} Bom cadastradas para o cnpj {cnpj}. Deseja cadastrar mais {quantidade}? (y/N)");
            if (Console.ReadKey(true).Key != ConsoleKey.Y) return;
        }

        var produtos = await Task.WhenAll((produtosSku.Length > 0)
            ? produtosSku.Select(sku => PersistirProdutoAsync(produtoClient, faker, sku))
            : Enumerable.Range(0, quantidade * 5).Select(i => GerarProdutoAsync(produtoClient, faker)));

        for (var i = 0; i < quantidade; i++)
        {
            var produto = await GerarProdutoAsync(produtoClient, faker);

            var bomBody = new CreateUpdateBomDto
            {
                EmpresaId = cliente.EmpresaId,
                ProdutoId = produto.Id
            };

            var parts = new List<CreateUpdateBomPartDto>();
            var available = produtos.OrderBy(q => faker.Random.Number()).ToList();

            for (var p = 0; p < faker.Random.Number(1, produtos.Length - 1); p++)
            {
                parts.Add(new CreateUpdateBomPartDto
                {
                    MaterialId = available[p].Id,
                    Quantidade = faker.Random.Number(1, 100)
                });
            }

            var bom = await bomClient.CreateAsync(bomBody, parts);
            Console.WriteLine($"Bom criada {bom.Id}");
        }
    }

    private static async Task<ProdutoDto> PersistirProdutoAsync(ProdutoApiClient produtoClient, Faker fake, string sku)
    {
        var search = await produtoClient.ListarAsync(new SearchProdutoRequest { BySku = sku });
        if (search.Any()) return search[0];

        return await produtoClient.CriarAsync(new CreateUpdateProdutoDto
        {
            Sku = sku,
            Descricao = fake.Commerce.ProductMaterial()
        });
    }

    private static async Task<ProdutoDto> GerarProdutoAsync(ProdutoApiClient produtoClient, Faker fake)
        => await produtoClient.CriarAsync(new CreateUpdateProdutoDto
        {
            Descricao = fake.Commerce.Product(),
            Sku = fake.Commerce.Ean13()
        });

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
