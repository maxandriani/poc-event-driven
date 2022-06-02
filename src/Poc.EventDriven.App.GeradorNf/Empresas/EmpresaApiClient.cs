using Poc.EventDriven.Api;
using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Empresas.Abstractions;

using System.Net.Http.Json;
using System.Text.Json;

namespace Poc.EventDriven.Empresas;

internal class EmpresaApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _serverUri;

    public EmpresaApiClient(string serverUri)
    {
        _serverUri = serverUri;
        _httpClient = new HttpClient();
    }

    public async Task<List<EmpresaDto>> ListarAsync(SearchEmpresaRequest? query = null)
    {
        var uri = new UriBuilder($"{_serverUri}/api/v1/empresa");

        if (query != null)
        {
            if (!string.IsNullOrEmpty(query.ByCnpj))
                uri.Query = $"ByCnpj={query.ByCnpj}";
        }

        var data = await _httpClient.GetStringAsync(uri.ToString());
        var response = JsonSerializer.Deserialize<CollectionResult<EmpresaDto>>(data, ApiOptions.JsonOptions);

        Console.WriteLine($"Foram encontrados {response.TotalCount} clientes.");

        return response.Items.ToList();
    }

    public async Task<EmpresaDto> CriarAsync(CreateUpdateEmpresaDto body)
    {
        var uri = new UriBuilder($"{_serverUri}/api/v1/empresa");
        var response = await _httpClient.PostAsync(uri.ToString(), JsonContent.Create(body, options: ApiOptions.JsonOptions));
        if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());
        return await response.Content.ReadFromJsonAsync<EmpresaDto>();
    }
}
