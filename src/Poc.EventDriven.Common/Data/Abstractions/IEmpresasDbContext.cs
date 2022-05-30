using Microsoft.EntityFrameworkCore;
using Poc.EventDriven.Empresas;

namespace Poc.EventDriven.Data.Abstractions;

public interface IEmpresasDbContext : IDbContext
{
    DbSet<Empresa> Empresas { get; }
}
