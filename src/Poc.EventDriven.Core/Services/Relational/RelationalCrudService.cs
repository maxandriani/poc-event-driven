using System.Text.Json;

using AutoMapper;

using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Services.Abstractions;

using Microsoft.Extensions.Logging;

namespace Poc.EventDriven.Services.Relational;


public abstract class RelationalCrudService<TDbContext, TEntity, TEntityDto, TKey, TSearchRequest, TCreateUpdateInput> :
    RelationalCrudService<TDbContext, TEntity, TEntityDto, TKey, TSearchRequest, TCreateUpdateInput, TCreateUpdateInput>
        where TDbContext : IDbContext
        where TEntity : class
        where TEntityDto : class
        where TKey : class
        where TSearchRequest : class
        where TCreateUpdateInput : class
{
    protected RelationalCrudService(ILogger logger, IMapper mapper, TDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }
}

public abstract class RelationalCrudService<TDbContext, TEntity, TEntityDto, TKey, TSearchRequest, TCreateInput, TUpdateInput> :
    RelationalReadOnlyService<TDbContext, TEntity, TEntityDto, TKey, TSearchRequest>,
    ICrudService<TEntityDto, TKey, TSearchRequest, TCreateInput, TUpdateInput>
        where TDbContext : IDbContext
        where TEntity : class
        where TEntityDto : class
        where TKey : class
        where TSearchRequest : class
        where TCreateInput : class
        where TUpdateInput : class
{
    protected RelationalCrudService(ILogger logger, IMapper mapper, TDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }

    private TEntity MapFromInput(TCreateInput input) => Mapper.Map<TCreateInput, TEntity>(input);
    private TEntity MapFromInput(TUpdateInput input, TEntity update) => Mapper.Map<TUpdateInput, TEntity>(input, update);

    protected Task ValidateAndThrowCreate(TEntity entity) => Task.CompletedTask;

    protected Task ValidateAndThrowUpdate(TEntity entity) => Task.CompletedTask;

    protected Task ValidateAndThrowDelete(TEntity entity) => Task.CompletedTask;

    public async Task<TEntityDto> CreateAsync(TCreateInput input, CancellationToken cancellationToken = default)
    {
        var entity = MapFromInput(input);
        await ValidateAndThrowCreate(entity);
        await DbContext.Set<TEntity>().AddAsync(entity);
        await DbContext.SaveChangesAsync();
        Logger.LogTrace($"{nameof(TEntity)} {JsonSerializer.Serialize(input)} salvo com sucesso!");

        return MapToOutput(entity);
    }

    public async Task<TEntityDto> UpdateAsync(TKey keys, TUpdateInput input)
    {
        var entity = MapFromInput(input, await GetEntityByIdAsync(keys));
        await ValidateAndThrowUpdate(entity);
        DbContext.Set<TEntity>().Update(entity);
        await DbContext.SaveChangesAsync();
        Logger.LogTrace($"{nameof(TEntity)} {JsonSerializer.Serialize(keys)} atualizado");
        
        return MapToOutput(entity);
    }

    public async Task DeleteAsync(TKey input)
    {
        var entity = await GetEntityByIdAsync(input);
        await ValidateAndThrowDelete(entity);
        DbContext.Set<TEntity>().Remove(entity);
        await DbContext.SaveChangesAsync();
        Logger.LogTrace($"{nameof(TEntity)} {JsonSerializer.Serialize(input)} removido com sucesso!");
    }
}