using Poc.EventDriven.Empresas;
using Poc.EventDriven.Nfs.Items;

namespace Poc.EventDriven.Nfs;

public class NfDocument
{
    public Guid Chave { get; set; }
    public int Numero { get; set; }
    public int Serie { get; set; }
    public EmpresaDocument? Empresa { get; set; }
    public EmpresaDocument? Exportador { get; set; }
    public EmpresaDocument? Emissor { get; set; }
    public DateTime Emissao { get; set; } = DateTime.Now;
    public TipoOperacao Operacao { get; set; }
    public List<NfItemDocument> Items { get; set; } = new();
}
