using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusDispatchConfigurationBuilder :
    IAzureServiceBusDispatchConfigurationBuilder
{

    private readonly string _clientName;

    public AzureServiceBusDispatchConfigurationBuilder(string clientName)
    {
        _clientName = clientName;
    }

    public IAzureServiceBusDispatchConfigurationBuilder WithEvent<TEvent>(string topicOrQueueName)
        where TEvent : class, IMessageBusEvent
    {
        ServiceBusDispatchConfigurations.Add<TEvent>(topicOrQueueName, _clientName);
        return this;
    }
}
