using Poc.EventDriven.Boms.Items.Abstractions;
using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Produtos.Abstractions;
using Poc.EventDriven.Services.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Boms.Abstractions;

public class BomDto
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public EmpresaDto? Empresa { get; set; }
    public Guid ProdutoId { get; set; }
    public ProdutoDto? Produto { get; set; }
}

public class CreateUpdateBomDto
{
    public Guid EmpresaId { get; set; }
    public Guid ProdutoId { get; set; }
}

public class SearchBomRequest : PagedAndSortedRequest
{
    public Guid? ByEmpresa { get; set; }
    public Guid? ByProduto { get; set; }
    public string? BySku { get; set; }
}
