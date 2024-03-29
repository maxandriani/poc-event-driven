﻿using Azure.Messaging.ServiceBus;

namespace Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

public interface IAzureServiceBusSettings
{
    string ClientName { get; }
    string QueueName { get; }
    string TopicName { get; }
    string SubscriptionName { get; }
    string[] SessionKeys { get; }

    ServiceBusProcessorOptions? ProcessorOptions { get; }
    ServiceBusReceiverOptions? ReceiverOptions { get; }
    ServiceBusSessionReceiverOptions? SessionReceiverOptions { get; }
    ServiceBusSessionProcessorOptions? SessionProcessorOptions { get; }

    void Deconstruct(out string queue, out string topic, out string subscription, out string[] sessionKeys);
}
