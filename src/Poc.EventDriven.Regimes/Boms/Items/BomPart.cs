using Poc.EventDriven.Produtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Boms.Items;

public class BomPart
{
    public Guid Id { get; set; }
    public Guid BomId { get; set; }
    public Bom? Bom { get; set; }
    public Guid MaterialId { get; set; }
    public Produto? Material { get; set; }
    public int Quantidade { get; set; }
}
