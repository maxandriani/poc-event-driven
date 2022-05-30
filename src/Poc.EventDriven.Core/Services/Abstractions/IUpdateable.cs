namespace Poc.EventDriven.Services.Abstractions;

public interface IUpdateable<TKey, TInput, TOutput>
    where TInput : class
    where TOutput : class
    where TKey : class
{
    Task<TOutput> UpdateAsync(TKey keys, TInput input);
}

public interface IUpdateable<TKey, TInput> : IUpdateable<TKey, TInput, TInput>
    where TKey : class
    where TInput: class
{ }