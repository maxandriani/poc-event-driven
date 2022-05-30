using AutoMapper;

using Microsoft.Extensions.Logging;

using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Empresas.Abstractions;
using Poc.EventDriven.Services.Relational;
using Poc.EventDriven.Services.Requests;
using Microsoft.EntityFrameworkCore;

namespace Poc.EventDriven.Empresas;

public class EmpresaApiService<TDbContext> :
    RelationalCrudService<TDbContext, Empresa, EmpresaDto, GetByKeyRequest<Guid>, SearchEmpresaRequest, CreateUpdateEmpresaDto>,
    IEmpresaApiService
        where TDbContext : IEmpresasDbContext
{
    public EmpresaApiService(
        ILogger<EmpresaApiService<TDbContext>> logger,
        IMapper mapper,
        TDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }

    protected override IQueryable<Empresa> CreateCollectionQuery(SearchEmpresaRequest input)
    {
        IQueryable<Empresa> query = DbContext.Empresas;

        if (!string.IsNullOrWhiteSpace(input.ByCnpj))
            query = query.Where(q => q.Cnpj.ToLower().StartsWith(input.ByCnpj.ToLower()));

        if (!string.IsNullOrWhiteSpace(input.ByNome))
            query = query.Where(q => q.Nome.ToLower().StartsWith(input.ByNome.ToLower()));

        return query;
    }

    protected override IQueryable<Empresa> DefaultSorting(IQueryable<Empresa> query)
        => query.OrderBy(q => q.Nome);

    protected override Task<Empresa> GetEntityByIdAsync(GetByKeyRequest<Guid> keys)
        => DbContext.Empresas.FirstAsync(q => q.Id == keys.Id);
}
