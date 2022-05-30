namespace Poc.EventDriven.Data.Abstractions;

/// <summary>
/// Um gerador de instâncias de DbContext não acoplado.
/// </summary>
/// <typeparam name="TContext"></typeparam>
public interface IRelationalDbContextFactory<TContext> where TContext : IDbContext
{
    /// <summary>
    /// Retorna uma nova instância (Scoped) de um IDbContext. 
    /// ATENÇÃO: Todas as instâncias geradas devem ser destruídas corretamente.
    /// <code>
    /// using (var context = contextFactory.CreateDbContext())...
    /// </code>
    /// </summary>
    /// <returns></returns>
    TContext CreateDbContext();

    /// <summary>
    /// Retorna uma nova instância (Scoped) de um IDbContext de forma assíncrona.
    /// ATENÇÃO: Todas as instâncias geradas devem ser destruídas corretamente.
    /// <code>
    /// using (var context = await contextFactory.CreateDbContextAsync())...
    /// </code>
    /// </summary>
    /// <returns></returns>
    Task<TContext> CreateDbContextAsync();
}

