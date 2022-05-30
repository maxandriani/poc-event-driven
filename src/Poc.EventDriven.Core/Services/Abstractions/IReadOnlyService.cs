using Poc.EventDriven.Services.Results;

namespace Poc.EventDriven.Services.Abstractions;

public interface IReadOnlyService<TEntityDto, TKey, TSearchRequest> :
  IReadable<TKey, TEntityDto>,
  ICollectionable<TSearchRequest, TEntityDto>
    where TSearchRequest : class
    where TEntityDto : class
    where TKey : class
{
}
