using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Poc.EventDriven.Clientes.Abstractions;
using Poc.EventDriven.Data.Abstractions;
using Poc.EventDriven.Services.Relational;
using Poc.EventDriven.Services.Requests;

namespace Poc.EventDriven.Clientes;

public class ClienteApiService :
    RelationalCrudService<IRegimesDbContext, Cliente, ClienteDto, GetByKeyRequest<Guid>, SearchClienteRequest, CreateUpdateClienteDto>,
    IClienteApiService
{
    public ClienteApiService(
        ILogger<ClienteApiService> logger,
        IMapper mapper,
        IRegimesDbContext dbContext) : base(logger, mapper, dbContext)
    {
    }

    protected override IQueryable<Cliente> CreateCollectionQuery(SearchClienteRequest input)
    {
        IQueryable<Cliente> query = DbContext.Clientes.Include(q => q.Empresa);

        if (!string.IsNullOrWhiteSpace(input.ByCnpj))
            query = query.Where(q => q.Empresa != null && q.Empresa.Cnpj.ToLower().StartsWith(input.ByCnpj.ToLower()));

        if (!string.IsNullOrWhiteSpace(input.ByName))
            query = query.Where(q => 
            q.Name.ToLower().StartsWith(input.ByName.ToLower()) 
            || q.Empresa != null && q.Empresa.Nome.ToLower().StartsWith(input.ByName.ToLower()));

        return query;
    }

    protected override IQueryable<Cliente> DefaultSorting(IQueryable<Cliente> query)
        => query.OrderBy(q => q.Name);

    protected override Task<Cliente> GetEntityByIdAsync(GetByKeyRequest<Guid> keys)
        => DbContext.Clientes.FirstAsync(q => q.Id == keys.Id);
}
