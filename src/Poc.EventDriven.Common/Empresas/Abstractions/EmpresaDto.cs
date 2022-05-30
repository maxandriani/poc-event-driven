using Poc.EventDriven.Services.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Empresas.Abstractions;

public class EmpresaDto
{
    public Guid Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

public class CreateUpdateEmpresaDto
{
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}

public class SearchEmpresaRequest : PagedAndSortedRequest
{
    public string? ByCnpj { get; set; }
    public string? ByNome { get; set; }
}