using Azure.Messaging.ServiceBus;

using Poc.EventDriven.MessageBus.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

public interface IAzureServiceBusDispatchSettings
{
    ServiceBusSender GetSender<TEvent>()
        where TEvent : class, IMessageBusEvent
            => GetSender(typeof(TEvent));

    ServiceBusSender GetSender(Type eventType);
}
