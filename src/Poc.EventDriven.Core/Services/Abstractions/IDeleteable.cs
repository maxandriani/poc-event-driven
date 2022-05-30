namespace Poc.EventDriven.Services.Abstractions;

public interface IDeleteable<TKeys>
    where TKeys : class
{
    Task DeleteAsync(TKeys input);
}