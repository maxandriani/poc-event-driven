using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Nfs.Items;

public class NfItemDocument
{
    public string Sku { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int ValorBrl { get; set; }
    public int ValorIcmsBrl { get; set; }
    public int ValorIcmsstBrl { get; set; }
    public int ValorIiBrl { get; set; }
    public int ValorIpiBrl { get; set; }
    public int ValorPisBrl { get; set; }
    public int ValorCofinsBrl { get; set; }
}
