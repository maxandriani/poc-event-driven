using Bogus;
using Bogus.Extensions.Brazil;

using Poc.EventDriven.Api;
using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.Empresas.Abstractions;

using System.Net.Http.Json;
using System.Text.Json;

namespace Poc.EventDriven.Clientes;

internal static class ClientesCmdHandler
{
    private static Faker faker = new Faker();

    private static async Task<EmpresaDto?> GerarEmpresa(EmpresaApiClient empresaApi, string? cnpj = null)
    {
        var data = new CreateUpdateEmpresaDto
        {
            Nome = faker.Company.CompanyName(),
            Cnpj = cnpj ?? faker.Company.Cnpj()
        };

        var checkResponse = await empresaApi.ListarAsync(new SearchEmpresaRequest { ByCnpj = data.Cnpj });
        if (checkResponse.Any())
            return checkResponse[0];

        return await empresaApi.CriarAsync(data);
    }

    private static async Task<ClienteDto> GerarCliente(ClienteApiClient clienteApi, EmpresaDto empresa)
    {
        var data = new CreateUpdateClienteDto
        {
            EmpresaId = empresa.Id,
            Name = empresa.Nome
        };

        var check = await clienteApi.ListarAsync(new SearchClienteRequest { ByCnpj = empresa.Cnpj });
        if (check.Any()) return check[0];

        var cliente = await clienteApi.CriarAsync(data);
        cliente.Empresa = empresa;

        return cliente;
    }

    public static async Task Gerar(string serverUri, int quantidade)
    {
        Console.WriteLine($"Criando {quantidade} clientes em {serverUri}.");

        var empresaApi = new EmpresaApiClient(serverUri);
        var clienteApi = new ClienteApiClient(serverUri);

        var empresas = await Task.WhenAll(Enumerable
            .Range(1, quantidade)
            .Select(i => GerarEmpresa(empresaApi)));

        var items = await Task.WhenAll(empresas
            .Where(x => x != null)
            .Select(empresa => GerarCliente(clienteApi, empresa)));

        foreach (var cliente in items)
        {
            Console.WriteLine($"Cnpj: {cliente.Empresa?.Cnpj}, Nome: {cliente.Name}, ClienteId: {cliente.Id}, EmpresaId: {cliente.EmpresaId}");
        }
    }

    public static async Task Gerar(string serverUri, string cnpj)
    {
        Console.WriteLine($"Criando um cliente {cnpj} em {serverUri}");

        var empresaApi = new EmpresaApiClient(serverUri);
        var clienteApi = new ClienteApiClient(serverUri);

        var empresa = await GerarEmpresa(empresaApi, cnpj);
        var cliente = await GerarCliente(clienteApi, empresa);

        Console.WriteLine($"Cnpj: {empresa.Cnpj}, Nome: {cliente.Name}, ClienteId: {cliente.Id}, EmpresaId: {empresa.Id}");
    }

    public static async Task Listar(string serverUri)
    {
        Console.WriteLine($"Listando clientes de {serverUri}");
        var clienteApi = new ClienteApiClient(serverUri);
        var clientes = await clienteApi.ListarAsync();

        foreach (var chunk in clientes.Chunk(20))
        {
            foreach(var item in chunk)
                Console.WriteLine($"Cnpj: {item.Empresa?.Cnpj}, Nome: {item.Name}");

            Console.WriteLine("Continuar listando? (y/n)");
            if (Console.ReadLine() == "y") break;
        }
    }
}
