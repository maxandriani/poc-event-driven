using Poc.EventDriven.Api;
using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Boms.Items.Abstractions;
using Poc.EventDriven.Produtos.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Boms;

public class ProdutoApiClient
{
    private readonly string _serverUri;
    private readonly HttpClient _httpClient;

    public ProdutoApiClient(string serverUri)
    {
        _serverUri = serverUri;
        _httpClient = new HttpClient();
    }

    public async Task<ProdutoDto> CriarAsync(CreateUpdateProdutoDto body)
    {
        var uri = new Uri($"{_serverUri}/api/v1/produto");
        var response = await _httpClient.PostAsync(uri, JsonContent.Create(body, options: ApiOptions.JsonOptions));
        if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());
        return await response.Content.ReadFromJsonAsync<ProdutoDto>(options: ApiOptions.JsonOptions);
    }

    public async Task<List<ProdutoDto>> ListarAsync(SearchProdutoRequest query)
    {
        var uri = new UriBuilder($"{_serverUri}/api/v1/produto");
        if (query != null)
        {
            var arguments = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(query.BySku))
                arguments["BySku"] = query.BySku;
            uri.Query = string.Join(",", arguments.Select((k, v) => $"{k}={v}"));
        }
        var response = await _httpClient.GetAsync(uri.Uri);
        if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());
        var items = await response.Content.ReadFromJsonAsync<CollectionResult<ProdutoDto>>(options: ApiOptions.JsonOptions);
        return items.Items.ToList();
    }

    public async Task<List<ProdutoDto>> ListarProdutosEmBomAsync(Guid[] bomIds)
    {
        var produtoReqs = await Task.WhenAll(bomIds.Select(bomId => _httpClient
            .GetFromJsonAsync<CollectionResult<BomPartDto>>($"{_serverUri}/api/v1/bom/{bomId}/item", options: ApiOptions.JsonOptions)));

        return produtoReqs
            .SelectMany(p => p.Items)
            .Select(p => p.Material)
            .ToList();
    }
}
