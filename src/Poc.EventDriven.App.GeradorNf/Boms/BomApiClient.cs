using Poc.EventDriven.Api;
using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Boms.Items.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Boms;

internal class BomApiClient
{
    private readonly string _serverUri;
    private readonly HttpClient _httpClient;

    public BomApiClient(string serverUri)
    {
        _serverUri = serverUri;
        _httpClient = new HttpClient();
    }

    public async Task<BomDto> CreateAsync(CreateUpdateBomDto bom, List<CreateUpdateBomPartDto> parts)
    {
        var bomUri = new Uri($"{_serverUri}/api/v1/bom");
        var request = await _httpClient.PostAsJsonAsync(bomUri, bom, options: ApiOptions.JsonOptions);
        if (!request.IsSuccessStatusCode) throw new Exception(await request.Content.ReadAsStringAsync());
        var createdBom = await request.Content.ReadFromJsonAsync<BomDto>(ApiOptions.JsonOptions);
        var childRequests = await Task.WhenAll(
            parts
                .Select(p =>
                {
                    p.BomId = createdBom.Id;
                    return p;
                })
                .Select(p => _httpClient.PostAsJsonAsync($"{_serverUri}/api/v1/bom/{createdBom.Id}/item", p, options: ApiOptions.JsonOptions)));
        if (childRequests.Where(r => r.IsSuccessStatusCode == false).Any()) Console.WriteLine($"Bom {createdBom.Id} foi criada, mas alguns itens não foram criados corretamente.");
        return createdBom;
    }

    public async Task<List<BomDto>> ListarAsync(SearchBomRequest? query = null)
    {
        var uri = new UriBuilder($"{_serverUri}/api/v1/bom");
        if (query != null)
        {
            var arguments = new Dictionary<string, string>();
            if (query.Take != null)
                arguments["Take"] = query.Take.ToString();
            if (query.ByProduto != null)
                arguments["ByProduto"] = query.ByProduto.ToString();
            if (query.ByEmpresa != null)
                arguments["ByEmpresa"] = query.ByEmpresa.ToString();

            uri.Query = string.Join(",", arguments.Select((k, v) => $"{k},{v}"));
        }

        var request = await _httpClient.GetAsync(uri.Uri);
        if (!request.IsSuccessStatusCode) throw new Exception(await request.Content.ReadAsStringAsync());
        var items = await request.Content.ReadFromJsonAsync<CollectionResult<BomDto>>(options: ApiOptions.JsonOptions);
        return items.Items.ToList();
    }

    public async Task<int> CountBomByClientId(Guid clienteId)
    {
        var uri = $"{_serverUri}/api/v1/bom?ByEmpresa={clienteId}&Take=1";
        var result = await _httpClient.GetFromJsonAsync<CollectionResult<BomDto>>(uri, ApiOptions.JsonOptions);
        return result.TotalCount;
    }
}
