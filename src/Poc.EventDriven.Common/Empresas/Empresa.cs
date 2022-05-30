using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Empresas;

public class Empresa
{
    public Guid Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
}
