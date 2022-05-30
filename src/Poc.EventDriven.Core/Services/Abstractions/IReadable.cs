namespace Poc.EventDriven.Services.Abstractions;

public interface IReadable<TKey, TOutput>
    where TKey : class
    where TOutput : class
{
    Task<TOutput> GetByIdAsync(TKey keys);
}