using Microsoft.EntityFrameworkCore;

using Poc.EventDriven.DwNf;
using Poc.EventDriven.DwNf.Items;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Data.Abstractions;

public interface IDwNfDbContext : IDbContext
{
    DbSet<DimEmpresa> DimEmpresas { get; }
    DbSet<DimTempo> DimTempo { get; }
    DbSet<DimTipoOperacao> DimTipoOperacoes { get; }
    DbSet<FactNf> FactNfs { get; }

    DbSet<DimNf> DimNfs { get; }
    DbSet<DimSku> DimSkus { get; }
    DbSet<FactNfItem> FactNfItems { get; }
}
