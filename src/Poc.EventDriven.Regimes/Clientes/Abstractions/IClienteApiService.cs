using Poc.EventDriven.Services.Abstractions;
using Poc.EventDriven.Services.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.Clientes.Abstractions;

public interface IClienteApiService : ICrudService<ClienteDto, GetByKeyRequest<Guid>, SearchClienteRequest, CreateUpdateClienteDto>
{
}
