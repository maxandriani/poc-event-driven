using Poc.EventDriven.Empresas;

namespace Poc.EventDriven.Clientes;

public class Cliente
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public string Name { get; set; } = string.Empty;
}
