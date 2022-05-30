using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Poc.EventDriven.Data.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Poc.EventDriven.Data;

public class RelationalDbContextFactory<TDbContext, TDbContextImplementation> : IRelationalDbContextFactory<TDbContext>
    where TDbContext : IDbContext
    where TDbContextImplementation : DbContext, TDbContext
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// https://github.com/dotnet/efcore/blob/main/src/EFCore/Extensions/EntityFrameworkServiceCollectionExtensions.cs
    /// https://github.com/dotnet/efcore/blob/main/src/EFCore/Internal/DbContextFactory.cs
    /// https://github.com/dotnet/efcore/blob/main/src/EFCore/Internal/DbContextFactorySource.cs
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="options"></param>
    /// <param name="factorySource"></param>
    public RelationalDbContextFactory(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TDbContext CreateDbContext() => (TDbContextImplementation) ActivatorUtilities.CreateInstance(_serviceProvider, typeof(TDbContextImplementation), Type.EmptyTypes);
    public Task<TDbContext> CreateDbContextAsync() => Task.FromResult(CreateDbContext());
}
