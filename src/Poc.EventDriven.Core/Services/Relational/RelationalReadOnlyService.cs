using System.Text.Json;
using AutoMapper;
using Poc.EventDriven.Services.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Services.Abstractions;
using Poc.EventDriven.Services.Requests.Abstractions;

namespace Poc.EventDriven.Services.Relational;

public abstract class RelationalReadOnlyService<TDbContext, TEntity, TEntityDto, TKey, TSearchRequest> :
    RelationalService<TDbContext>,
    IReadOnlyService<TEntityDto, TKey, TSearchRequest>
    where TDbContext : IDbContext
    where TEntity : class
    where TEntityDto : class
    where TKey : class
    where TSearchRequest : class
{
    protected RelationalReadOnlyService(ILogger logger, IMapper mapper, TDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }

    protected abstract Task<TEntity> GetEntityByIdAsync(TKey keys);

    protected TEntityDto MapToOutput(TEntity entity) => Mapper.Map<TEntity, TEntityDto>(entity);

    public async Task<TEntityDto> GetByIdAsync(TKey keys)
    {
        var place = await GetEntityByIdAsync(keys);
        Logger.LogTrace($"{nameof(TEntity)} {JsonSerializer.Serialize(keys)} foi consultado");
        return MapToOutput(place);
    }

    protected abstract IQueryable<TEntity> CreateCollectionQuery(TSearchRequest input);

    protected abstract IQueryable<TEntity> DefaultSorting(IQueryable<TEntity> query);

    public async Task<CollectionResult<TEntityDto>> GetCollectionAsync(TSearchRequest input)
    {
        IQueryable<TEntity> query = CreateCollectionQuery(input);

        var totalCount = await query.CountAsync();

        if (input is IPagedRequest pagedInput)
            query = ApplyPagination(query, pagedInput);

        if (input is ISortedRequest sortedInput)
            query = ApplySorting(query, sortedInput);

        var result = query.AsEnumerable<TEntity>().Select(p => MapToOutput(p));

        Logger.LogTrace($"Consulta de Places realizada com os seguintes argumentos: {JsonSerializer.Serialize(input)}");

        return new CollectionResult<TEntityDto>(result, totalCount);
    }

    private IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, ISortedRequest? input)
    {
        if (string.IsNullOrEmpty(input?.Sorting)) return DefaultSorting(query);
        return query.OrderBy(input.Sorting);
    }

    private IQueryable<TEntity> ApplyPagination(IQueryable<TEntity> query, IPagedRequest? input)
    {
        if (input?.Take < 1 || input?.Page < 0) return query;

        var take = input?.Take ?? 0;
        var skip = (input?.Page ?? 0) * take;

        return query.Skip(skip).Take(take);
    }
}