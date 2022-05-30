namespace Poc.EventDriven.Services.Abstractions;

public interface ICreateable<TCreate, TOutput>
    where TCreate : class
    where TOutput : class
{
    Task<TOutput> CreateAsync(TCreate input, CancellationToken cancellationToken = default);
}

public interface ICreateable<TCreate> : ICreateable<TCreate, TCreate>
    where TCreate : class { }