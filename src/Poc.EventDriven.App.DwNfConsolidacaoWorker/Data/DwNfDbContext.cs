using Microsoft.EntityFrameworkCore;

using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.DwNf;
using Poc.EventDriven.DwNf.Items;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Data;

public class DwNfDbContext : DbContext, IDwNfDbContext
{
    public DwNfDbContext(DbContextOptions<DwNfDbContext> options) : base(options)
    {
    }

    public DbSet<DimEmpresa> DimEmpresas => Set< DimEmpresa>();

    public DbSet<DimTempo> DimTempo => Set<DimTempo>();

    public DbSet<DimTipoOperacao> DimTipoOperacoes => Set<DimTipoOperacao>();

    public DbSet<FactNf> FactNfs => Set<FactNf>();

    public DbSet<DimNf> DimNfs => Set<DimNf>();

    public DbSet<DimSku> DimSkus => Set<DimSku>();

    public DbSet<FactNfItem> FactNfItems => Set<FactNfItem>();

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);

    //    modelBuilder.Entity<DimTempo>()
    //        .Property(p => p.Date)
    //            .HasColumnType("timestamp without time zone");
    //}
}
