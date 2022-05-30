using Poc.EventDriven.Services.Abstractions;
using Poc.EventDriven.Services.Requests;

namespace Poc.EventDriven.Produtos.Abstractions;

public interface IProdutoApiService : ICrudService<ProdutoDto, GetByKeyRequest<Guid>, SearchProdutoRequest, CreateUpdateProdutoDto>
{
}
