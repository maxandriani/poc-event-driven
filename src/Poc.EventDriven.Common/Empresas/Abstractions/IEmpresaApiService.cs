using Poc.EventDriven.Services.Abstractions;
using Poc.EventDriven.Services.Requests;

namespace Poc.EventDriven.Empresas.Abstractions;

public interface IEmpresaApiService : ICrudService<EmpresaDto, GetByKeyRequest<Guid>, SearchEmpresaRequest, CreateUpdateEmpresaDto>
{
}
