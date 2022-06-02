using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Azure;

using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

internal class ServiceBusDispatchConfigurations : 
    IAzureServiceBusDispatchSettings
{
    private static readonly Dictionary<Type, (string QueueOrTopic, string ClientName)> _eventSettings = new();
    private readonly IAzureClientFactory<ServiceBusClient> _azureClientFactory;

    public ServiceBusDispatchConfigurations(
        IAzureClientFactory<ServiceBusClient> azureClientFactory)
    {
        _azureClientFactory = azureClientFactory;
    }

    internal static void Add<TEvent>(string queueOrTopic, string client)
        where TEvent : class, IMessageBusEvent
            => Add(typeof(TEvent), queueOrTopic, client);

    internal static void Add(Type type, string queueOrTopic, string client)
    {
        _eventSettings[type] = (queueOrTopic, client);
    }

    public ServiceBusSender GetSender(Type eventType)
    {
        var (topicName, clientName) = _eventSettings[eventType];
        return _azureClientFactory
            .CreateClient(clientName)
            .CreateSender(topicName);
    }
}
