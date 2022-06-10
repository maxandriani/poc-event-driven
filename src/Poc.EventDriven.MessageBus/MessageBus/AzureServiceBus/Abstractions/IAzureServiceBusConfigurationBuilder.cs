using Azure.Messaging.ServiceBus;

using System;

namespace Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

public interface IAzureServiceBusConfigurationBuilder
{
    IAzureServiceBusConfigurationBuilder WithClientName(string clientName);
    IAzureServiceBusConfigurationBuilder WithQueueName(string queueName);
    IAzureServiceBusConfigurationBuilder WithTopicSubscription(string topicName, string subscriptionName);
    IAzureServiceBusConfigurationBuilder WithProcessorOptions(Action<ServiceBusProcessorOptions> processorOptionsAction);
    IAzureServiceBusConfigurationBuilder WithReceiverOptions(Action<ServiceBusReceiverOptions> receiverOptionsAction);
    IAzureServiceBusConfigurationBuilder WithSessionReceiverOptions(Action<ServiceBusSessionReceiverOptions> receiverOptionsAction);

    IAzureServiceBusConfigurationBuilder WithSessionReceiverOptions(string[] sessionKeys, Action<ServiceBusSessionReceiverOptions> receiverOptionsAction);
}
