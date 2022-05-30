using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

namespace Poc.EventDriven.MessageBus.Abstractions;

public interface IMessageBusBatchEventHandler<TEvent>
    where TEvent : class, IMessageBusEvent
{
    Task HandleBatchAsync(List<MessageWithBag<TEvent>> batch, CancellationToken cancellationToken = default);
}