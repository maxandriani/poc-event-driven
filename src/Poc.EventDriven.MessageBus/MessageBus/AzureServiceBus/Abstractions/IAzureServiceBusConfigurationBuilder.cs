using Azure.Messaging.ServiceBus;

namespace Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

public interface IAzureServiceBusConfigurationBuilder
{
    IAzureServiceBusConfigurationBuilder WithConnectionString(string connectionString);
    IAzureServiceBusConfigurationBuilder WithQueueName(string queueName);
    IAzureServiceBusConfigurationBuilder WithTopicSubscription(string topicName, string subscriptionName);
    IAzureServiceBusConfigurationBuilder WithClientOptions(Action<ServiceBusClientOptions> clientOptionsAction);
    IAzureServiceBusConfigurationBuilder WithProcessorOptions(Action<ServiceBusProcessorOptions> processorOptionsAction);
    IAzureServiceBusConfigurationBuilder WithReceiverOptions(Action<ServiceBusReceiverOptions> receiverOptionsAction);
    IAzureServiceBusConfigurationBuilder WithSessionReceiverOptions(Action<ServiceBusSessionReceiverOptions> receiverOptionsAction);

    IAzureServiceBusConfigurationBuilder WithSessionReceiverOptions(string[] sessionKeys, Action<ServiceBusSessionReceiverOptions> receiverOptionsAction);
}
