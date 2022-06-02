using Poc.EventDriven.Api;
using Poc.EventDriven.Clientes.Abstractions;
using System.Net.Http.Json;

namespace Poc.EventDriven.Clientes;

internal class ClienteApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _serverUri;

    public ClienteApiClient(string serverUri)
    {
        _serverUri = serverUri;
        _httpClient = new HttpClient();
    }

    public async Task<ClienteDto> CriarAsync(CreateUpdateClienteDto create)
    {
        var uri = new UriBuilder($"{_serverUri}/api/v1/cliente");
        var response = await _httpClient.PostAsync(uri.ToString(), JsonContent.Create(create, options: ApiOptions.JsonOptions));
        if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());
        return await response.Content.ReadFromJsonAsync<ClienteDto>();
    }

    public async Task<List<ClienteDto>> ListarAsync(SearchClienteRequest? query = null)
    {
        var uri = new UriBuilder($"{_serverUri}/api/v1/cliente");
        if (query != null)
        {
            var arguments = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(query.ByCnpj))
                arguments["ByCnpj"] = query.ByCnpj;

            if (!string.IsNullOrEmpty(query.ByName))
                arguments["ByName"] = query.ByName;

            uri.Query = string.Join(",", arguments.Select((k, v) => $"{k}={v}"));
        }
        var response = await _httpClient.GetAsync(uri.ToString());
        if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());
        var clienteCollection = await response.Content.ReadFromJsonAsync<CollectionResult<ClienteDto>>(options : ApiOptions.JsonOptions);
        return clienteCollection.Items.ToList();
    }
}
