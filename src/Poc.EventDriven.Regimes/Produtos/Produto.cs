using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Produtos;

public class Produto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}
