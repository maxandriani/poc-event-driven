using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Services.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Clientes.Abstractions;

public class ClienteDto
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public EmpresaDto? Empresa { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateUpdateClienteDto
{
    public Guid EmpresaId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SearchClienteRequest : PagedAndSortedRequest
{
    public string? ByCnpj { get; set; }
    public string? ByName { get; set; }
}