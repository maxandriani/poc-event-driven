using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Logging;

using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

using System.Text;
using System.Text.Json;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

public class AzureServiceBusDispatcher : IMessageBusDispatcher
{
    private readonly IAzureServiceBusDispatchSettings _dispatcherSettings;
    private readonly ILogger<AzureServiceBusDispatcher> _logger;

    public AzureServiceBusDispatcher(
        ILogger<AzureServiceBusDispatcher> logger,
        IAzureServiceBusDispatchSettings dispatcherSettings)
    {
        _logger = logger;
        _dispatcherSettings = dispatcherSettings;
    }

    public async Task DispatchAsync<TEvent>(
        TEvent eventMessage,
        string? sessionId = null,
        string? partitionKey = null,
        CancellationToken cancellationToken = default)
        where TEvent : class, IMessageBusEvent
    {
        var message = new ServiceBusMessage(EncodeMessage(eventMessage));

        if (sessionId != null)
            message.SessionId = sessionId;

        if (partitionKey != null)
            message.PartitionKey = partitionKey;

        var sender = _dispatcherSettings.GetSender<TEvent>();

        await sender.SendMessageAsync(message, cancellationToken);
        _logger.LogTrace($"Message sent {message.MessageId}", eventMessage);
    }

    public async Task DispatchAsync<TEvent>(
        TEvent eventMessage,
        Dictionary<string, object> properties,
        string? sessionId = null,
        string? partitionKey = null, 
        CancellationToken cancellationToken = default)
        where TEvent : class, IMessageBusEvent
    {
        var message = new ServiceBusMessage(EncodeMessage(eventMessage));

        foreach(var property in properties)
            message.ApplicationProperties.Add(property);

        if (sessionId != null)
            message.SessionId = sessionId;

        if (partitionKey != null)
            message.PartitionKey = partitionKey;

        var sender = _dispatcherSettings.GetSender<TEvent>();

        await sender.SendMessageAsync(message, cancellationToken);
        _logger.LogTrace($"Message sent {message.MessageId}", eventMessage);
    }

    private byte[] EncodeMessage<TEvent>(TEvent body)
        where TEvent : class, IMessageBusEvent
    {
        var jsonBody = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return Encoding.UTF8.GetBytes(jsonBody);
    }
}
