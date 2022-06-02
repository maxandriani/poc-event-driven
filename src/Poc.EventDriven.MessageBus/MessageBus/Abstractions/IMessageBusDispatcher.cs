namespace Poc.EventDriven.MessageBus.Abstractions;

public interface IMessageBusDispatcher
{
    Task DispatchAsync<TEvent>(
        TEvent eventMessage,
        string? sessionId = null,
        string? partitionKey = null,
        CancellationToken cancellationToken = default)
            where TEvent : class, IMessageBusEvent;

    Task DispatchAsync<TEvent>(
        TEvent eventMessage,
        Dictionary<string, object> properties,
        string? sessionId = null,
        string? partitionKey = null,
        CancellationToken cancellationToken = default)
            where TEvent: class, IMessageBusEvent;
}
