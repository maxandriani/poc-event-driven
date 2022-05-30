using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Poc.EventDriven.Boms.Abstractions;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Services.Relational;
using Poc.EventDriven.Services.Requests;

namespace Poc.EventDriven.Boms;

public class BomApiService :
    RelationalCrudService<IRegimesDbContext, Bom, BomDto, GetByKeyRequest<Guid>, SearchBomRequest, CreateUpdateBomDto>,
    IBomApiService
{
    public BomApiService(
        ILogger<BomApiService> logger,
        IMapper mapper,
        IRegimesDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }

    protected override IQueryable<Bom> CreateCollectionQuery(SearchBomRequest input)
    {
        IQueryable<Bom> query = DbContext.Boms
            .Include(q => q.Empresa)
            .Include(q => q.Produto);

        if (input.ByEmpresa.HasValue)
            query = query.Where(q => q.EmpresaId == input.ByEmpresa);

        if (input.ByProduto.HasValue)
            query = query.Where(q => q.ProdutoId == input.ByProduto);

        if (!string.IsNullOrWhiteSpace(input.BySku))
            query = query.Where(q => q.Produto!.Sku.ToLower().StartsWith(input.BySku.ToLower()));

        return query;
    }

    protected override IQueryable<Bom> DefaultSorting(IQueryable<Bom> query)
        => query.OrderBy(q => q.Empresa!.Nome);

    protected override Task<Bom> GetEntityByIdAsync(GetByKeyRequest<Guid> keys)
        => DbContext.Boms.FirstAsync(q => q.Id == keys.Id);
}
