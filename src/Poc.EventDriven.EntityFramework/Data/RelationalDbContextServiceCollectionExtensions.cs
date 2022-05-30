using Poc.EventDriven.Data;
using Poc.EventDriven.Data.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Adiciona novas extensões para DbContexts não acoplados.
/// </summary>
public static class RelationalDbContextServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona uma referência de DbContext desacoplada.
    /// Adiciona um Factory IRelationalDbContextFactory para o tipo DbContext
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="DbContextImplementation"></typeparam>
    /// <param name="serviceCollection"></param>
    /// <param name="optionsAction"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public static IServiceCollection AddAppDbContextFactory<TDbContext, DbContextImplementation>(
        this IServiceCollection serviceCollection,
        Action<DbContextOptionsBuilder>? optionsAction = null,
        ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TDbContext : IDbContext
        where DbContextImplementation : DbContext, TDbContext
    {
        serviceCollection.AddDbContext<TDbContext, DbContextImplementation>(optionsAction); // AddDbContext also calls AddCoreServices w/ inject DbContextOptions<TImplementation>
        serviceCollection.Add(new ServiceDescriptor(
            typeof(IRelationalDbContextFactory<TDbContext>),
            typeof(RelationalDbContextFactory<TDbContext, DbContextImplementation>),
            lifetime));

        return serviceCollection;
    }
}
