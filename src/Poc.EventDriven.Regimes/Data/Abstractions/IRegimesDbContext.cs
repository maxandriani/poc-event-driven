using Microsoft.EntityFrameworkCore;

using Poc.EventDriven.Boms;
using Poc.EventDriven.Boms.Items;
using Poc.EventDriven.Clientes;
using Poc.EventDriven.Produtos;

namespace Poc.EventDriven.Data.Abstractions;

public interface IRegimesDbContext : IEmpresasDbContext, IDbContext
{
    DbSet<Cliente> Clientes { get; }
    DbSet<Produto> Produtos { get; }
    DbSet<Bom> Boms { get; }
    DbSet<BomPart> BomParts { get; }
}
