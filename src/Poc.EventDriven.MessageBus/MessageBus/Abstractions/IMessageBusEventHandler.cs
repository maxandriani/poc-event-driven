namespace Poc.EventDriven.MessageBus.Abstractions;

public interface IMessageBusEventHandler<TEvent>
    where TEvent : class, IMessageBusEvent
{
    Task HandleAsync(TEvent body, IMessageBag messageBag, CancellationToken cancellationToken = default);
}
