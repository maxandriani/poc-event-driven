using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Poc.EventDriven.Boms.Items.Abstractions;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Services.Relational;

namespace Poc.EventDriven.Boms.Items;

public class BomPartApiService :
    RelationalCrudService<IRegimesDbContext, BomPart, BomPartDto, GetBomPartByKey, SearchBomPartRequest, CreateUpdateBomPartDto>,
    IBomPartApiService
{
    public BomPartApiService(
        ILogger<BomPartApiService> logger,
        IMapper mapper,
        IRegimesDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }

    protected override IQueryable<BomPart> CreateCollectionQuery(SearchBomPartRequest input)
    {
        IQueryable<BomPart> query = DbContext
            .BomParts
            .Include(q => q.Material)
            .Where(q => q.BomId == input.BomId);

        if (input.ByProduto.HasValue)
            query = query.Where(q => q.MaterialId == input.ByProduto);

        if (!string.IsNullOrWhiteSpace(input.BySku))
            query = query.Where(q => q.Material!.Sku.ToLower().StartsWith(input.BySku!.ToLower()));

        return query;
    }

    protected override IQueryable<BomPart> DefaultSorting(IQueryable<BomPart> query)
        => query.OrderBy(q => q.Material!.Sku);

    protected override Task<BomPart> GetEntityByIdAsync(GetBomPartByKey keys)
        => DbContext.BomParts.FirstAsync(q => q.Id == keys.Id && q.BomId == keys.BomId);
}
