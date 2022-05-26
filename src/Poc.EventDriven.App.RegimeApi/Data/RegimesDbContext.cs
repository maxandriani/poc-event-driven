using Microsoft.EntityFrameworkCore;

using Poc.EventDriven.Boms;
using Poc.EventDriven.Boms.Items;
using Poc.EventDriven.Clientes;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Empresas;
using Poc.EventDriven.Produtos;

namespace Poc.EventDriven.Data;

public class RegimesDbContext : DbContext, IRegimesDbContext
{
    public RegimesDbContext(DbContextOptions<RegimesDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<Produto> Produtos => Set<Produto>();

    public DbSet<Bom> Boms => Set<Bom>();

    public DbSet<BomPart> BomParts => Set<BomPart>();

    public DbSet<Empresa> Empresas => Set<Empresa>();

}
