using AutoMapper;

using Poc.EventDriven.Data.Abstractions;

using Microsoft.Extensions.Logging;

namespace Poc.EventDriven.Services.Relational;

public abstract class RelationalService<TDbContext>
    where TDbContext : IDbContext
{
    protected ILogger Logger;
    protected IMapper Mapper;
    protected TDbContext DbContext;

    protected RelationalService(ILogger logger, IMapper mapper, TDbContext dbContext)
    {
        Logger = logger;
        Mapper = mapper;
        DbContext = dbContext;
    }

}