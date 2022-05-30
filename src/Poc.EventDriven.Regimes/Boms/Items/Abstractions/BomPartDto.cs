using Poc.EventDriven.Produtos.Abstractions;
using Poc.EventDriven.Services.Requests;

namespace Poc.EventDriven.Boms.Items.Abstractions;

public class BomPartDto
{
    public Guid Id { get; set; }
    public Guid BomId { get; set; }
    public Guid MaterialId { get; set; }
    public ProdutoDto? Material { get; set; }
    public int Quantidade { get; set; }
}

public class CreateUpdateBomPartDto
{
    public Guid BomId { get; set; }
    public Guid MaterialId { get; set; }
    public int Quantidade { get; set; }
}

public class SearchBomPartRequest : PagedAndSortedRequest
{
    public Guid BomId { get; set; }
    public Guid? ByProduto { get; set; }
    public string? BySku { get; set; }
}

public class GetBomPartByKey
{
    public Guid Id { get; set; }
    public Guid BomId { get; set; }
}