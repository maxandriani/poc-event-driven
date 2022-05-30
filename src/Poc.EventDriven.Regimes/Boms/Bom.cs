using Poc.EventDriven.Boms.Items;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.Produtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Boms;

public class Bom
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public Guid ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public List<BomPart> Parts { get; set; } = new();
}
