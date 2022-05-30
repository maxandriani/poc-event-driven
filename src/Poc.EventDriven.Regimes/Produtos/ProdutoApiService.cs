using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Produtos.Abstractions;
using Poc.EventDriven.Services.Relational;
using Poc.EventDriven.Services.Requests;

using System.Linq;

namespace Poc.EventDriven.Produtos;

public class ProdutoApiService :
    RelationalCrudService<IRegimesDbContext, Produto, ProdutoDto, GetByKeyRequest<Guid>, SearchProdutoRequest, CreateUpdateProdutoDto>,
    IProdutoApiService
{
    public ProdutoApiService(
        ILogger<ProdutoApiService> logger,
        IMapper mapper,
        IRegimesDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }

    protected override IQueryable<Produto> CreateCollectionQuery(SearchProdutoRequest input)
    {
        IQueryable<Produto> produtos = DbContext.Produtos;

        if (!string.IsNullOrWhiteSpace(input.BySku))
            produtos = produtos.Where(q => q.Sku.ToLower().StartsWith(input.BySku!.ToLower()));

        return produtos;
    }

    protected override IQueryable<Produto> DefaultSorting(IQueryable<Produto> query)
        => query.OrderBy(q => q.Descricao);

    protected override Task<Produto> GetEntityByIdAsync(GetByKeyRequest<Guid> keys)
        => DbContext.Produtos.FirstAsync(q => q.Id == keys.Id);
}
