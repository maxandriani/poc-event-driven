namespace Poc.EventDriven.DwNf.Items;

public class DimNf
{
    public int Id { get; set; }
    public Guid Chave { get; set; }
    public int Numero { get; set; }
    public int Serie { get; set; }
    public string BlobAddress { get; set; } = string.Empty;
}
