using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusBatchHostedService<TEvent> : IMessageBusManager
    where TEvent : class, IMessageBusEvent
{
    private readonly ILogger<AzureServiceBusBatchHostedService<TEvent>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusReceiver _receiver;
    private bool _hasStarted = false;
    private CancellationTokenSource? _stoppingTokenSource;

    public AzureServiceBusBatchHostedService(
        ILogger<AzureServiceBusBatchHostedService<TEvent>> logger,
        IServiceProvider serviceProvider,
        IOptions<IAzureServiceBusSettings> _options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _client = new ServiceBusClient(_options.Value.ConnectionString, _options.Value.ClientOptions);
        var (queue, topic, subscription, _) = _options.Value;

        _receiver = (!string.IsNullOrWhiteSpace(queue), !string.IsNullOrWhiteSpace(topic), !string.IsNullOrWhiteSpace(subscription)) switch
        {
            (true, false, false) => _client.CreateReceiver(queue, _options.Value.ReceiverOptions),
            (false, true, true) => _client.CreateReceiver(topic, subscription, _options.Value.ReceiverOptions),
            (_, _, _) => throw new ArgumentException("Você precisa informar uma Queue ou um conjunto de Topic/Subscription, mas não ambos.")
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_hasStarted) return Task.CompletedTask;
        _hasStarted = true;
        _stoppingTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Task.Run(() => ExecuteAsync(cancellationToken));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_hasStarted)
            _stoppingTokenSource?.Cancel();
        
        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            List<MessageWithBag<TEvent>> payload = new();
            
            try
            {
                var receivedMessages = await _receiver.ReceiveMessagesAsync(
                    maxMessages: 1024,
                    maxWaitTime: TimeSpan.FromMinutes(5),
                    cancellationToken);

                if (receivedMessages == null) continue;

                payload = receivedMessages
                .Select(x => new MessageWithBag<TEvent>(
                    x.Body.ToObjectFromJson<TEvent>(),
                    new AzureServiceBusBatchMessageBag<TEvent>(x, _receiver)))
                .ToList();
                _logger.LogInformation($"New chunk of {receivedMessages.Count} messages received.");

                // Get processor instance...
                using IServiceScope scope = _serviceProvider.CreateScope();

                var handler = scope.ServiceProvider.GetRequiredService<IMessageBusBatchEventHandler<TEvent>>();
                await handler.HandleBatchAsync(payload, _stoppingTokenSource!.Token);
            }
            catch (Exception ex)
            {
                var resolving = new List<Task>();

                foreach (var m in payload)
                {
                    if (!m.Bag.Resolved) resolving.Add(m.Bag.AbortAsync(ex));
                    _logger.LogError(ex.Message, ex, ex.Message);
                }

                await Task.WhenAll(resolving);
            }
            finally
            {
                var resolving = new List<Task>();

                foreach (var m in payload)
                    if (!m.Bag.Resolved) resolving.Add(m.Bag.CompleteAsync());

                await Task.WhenAll(resolving);

                payload.Clear();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _stoppingTokenSource?.Cancel();
        await _client.DisposeAsync();
        await _receiver.DisposeAsync();
    }

    public void Dispose() => DisposeAsync().GetAwaiter();
}
