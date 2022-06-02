using Poc.EventDriven.MessageBus.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

public interface IAzureServiceBusDispatchConfigurationBuilder
{
    IAzureServiceBusDispatchConfigurationBuilder WithEvent<TEvent>(string topicOrQueueName)
        where TEvent : class, IMessageBusEvent;
}
