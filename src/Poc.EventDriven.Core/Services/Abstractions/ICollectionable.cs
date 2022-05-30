using Poc.EventDriven.Services.Results;

namespace Poc.EventDriven.Services.Abstractions;

public interface ICollectionable<TGetAllRequest, TOutput>
    where TGetAllRequest : class
    where TOutput : class
{
    Task<CollectionResult<TOutput>> GetCollectionAsync(TGetAllRequest input);
}