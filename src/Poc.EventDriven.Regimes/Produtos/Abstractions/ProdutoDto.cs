using Poc.EventDriven.Services.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Produtos.Abstractions;

public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}

public class CreateUpdateProdutoDto
{
    public string Sku { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}

public class SearchProdutoRequest : PagedAndSortedRequest
{
    public string? BySku { get; set; }
}