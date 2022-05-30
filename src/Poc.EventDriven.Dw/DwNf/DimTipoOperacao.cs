using Poc.EventDriven.Nfs;

namespace Poc.EventDriven.DwNf;

public class DimTipoOperacao
{
    public int Id { get; set; }
    public TipoOperacao TipoOperacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
