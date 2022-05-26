namespace Poc.EventDriven.V1.Boms.Items;

public class BomPartCreateUpdateBody
{
    public Guid MaterialId { get; set; }
    public int Quantidade { get; set; }
}
