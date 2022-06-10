using Azure.Messaging.ServiceBus;

using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusConfigurationBuilder :
    IAzureServiceBusSettings,
    IAzureServiceBusConfigurationBuilder
{
    public string ClientName { get; private set; } = string.Empty;

    public string QueueName { get; private set; } = string.Empty;

    public string TopicName { get; private set; } = string.Empty;

    public string SubscriptionName { get; private set; } = string.Empty;

    public ServiceBusProcessorOptions? ProcessorOptions { get; private set; }

    public ServiceBusReceiverOptions? ReceiverOptions { get; private set; }

    public string[] SessionKeys { get; private set; } = Array.Empty<string>();

    public ServiceBusSessionReceiverOptions? SessionReceiverOptions { get; private set; }

    public ServiceBusSessionProcessorOptions? SessionProcessorOptions { get; private set; }

    public void Deconstruct(out string queue, out string topic, out string subscription, out string[] sessionKeys)
    {
        queue = QueueName;
        topic = TopicName;
        subscription = SubscriptionName;
        sessionKeys = SessionKeys;
    }
    
    public IAzureServiceBusConfigurationBuilder WithClientName(string clientName)
    {
        ClientName = clientName;
        return this;
    }

    public IAzureServiceBusConfigurationBuilder WithProcessorOptions(Action<ServiceBusProcessorOptions> processorOptionsAction)
    {
        if (ProcessorOptions == null)
            ProcessorOptions = new ServiceBusProcessorOptions();

        processorOptionsAction(ProcessorOptions);

        return this;
    }

    public IAzureServiceBusConfigurationBuilder WithReceiverOptions(Action<ServiceBusReceiverOptions> receiverOptionsAction)
    {
        if (ReceiverOptions == null)
            ReceiverOptions = new ServiceBusReceiverOptions();

        receiverOptionsAction(ReceiverOptions);

        return this;
    }

    public IAzureServiceBusConfigurationBuilder WithQueueName(string queueName)
    {
        QueueName = queueName;
        return this;
    }

    public IAzureServiceBusConfigurationBuilder WithTopicSubscription(string topicName, string subscriptionName)
    {
        TopicName = topicName;
        SubscriptionName = subscriptionName;
        return this;
    }

    public IAzureServiceBusConfigurationBuilder WithSessionReceiverOptions(Action<ServiceBusSessionReceiverOptions> receiverOptionsAction)
    {
        if (SessionReceiverOptions == null)
            SessionReceiverOptions = new ServiceBusSessionReceiverOptions();

        receiverOptionsAction(SessionReceiverOptions);

        return this;
    }

    public IAzureServiceBusConfigurationBuilder WithSessionReceiverOptions(string[] sessionKeys, Action<ServiceBusSessionReceiverOptions> receiverOptionsAction)
    {
        if (SessionReceiverOptions == null)
            SessionReceiverOptions = new ServiceBusSessionReceiverOptions();

        SessionKeys = SessionKeys;
        receiverOptionsAction(SessionReceiverOptions);

        return this;
    }
}
