using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Poc.EventDriven.HealthChecks.Abstractions;
using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

using System.Text.Json;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusBatchHostedService<TEvent> : IMessageBusManager
    where TEvent : class, IMessageBusEvent
{
    private readonly ILogger<AzureServiceBusBatchHostedService<TEvent>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWatchDogFactory _watchDogFactory;
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;
    private readonly IOptions<IAzureServiceBusSettings> _options;
    private IWatchDog? _watchDog = null;
    private bool _hasStarted = false;
    private CancellationTokenSource? _stoppingTokenSource;

    public AzureServiceBusBatchHostedService(
        ILogger<AzureServiceBusBatchHostedService<TEvent>> logger,
        IServiceProvider serviceProvider,
        IWatchDogFactory wathcDogFactory,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IOptions<IAzureServiceBusSettings> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _watchDogFactory = wathcDogFactory;
        _serviceBusClientFactory = serviceBusClientFactory;
        _options = options;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_hasStarted) return Task.CompletedTask;
        _hasStarted = true;
        _stoppingTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _watchDog = _watchDogFactory.CreateTimeoutWatchDog(TimeSpan.FromMinutes(15), $"O processamento do evento {nameof(TEvent)} não está respondendo.");
        Task.Run(() => ExecuteAsync(cancellationToken));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_hasStarted)
            _stoppingTokenSource?.Cancel();

        _watchDog?.Dispose();
        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var client = _serviceBusClientFactory.CreateClient(_options.Value.ClientName);
        var (queue, topic, subscription, _) = _options.Value;

        await using var receiver = (!string.IsNullOrWhiteSpace(queue), !string.IsNullOrWhiteSpace(topic), !string.IsNullOrWhiteSpace(subscription)) switch
        {
            (true, false, false) => client.CreateReceiver(queue, _options.Value.ReceiverOptions),
            (false, true, true) => client.CreateReceiver(topic, subscription, _options.Value.ReceiverOptions),
            (_, _, _) => throw new ArgumentException("Você precisa informar uma Queue ou um conjunto de Topic/Subscription, mas não ambos.")
        };

        while (cancellationToken.IsCancellationRequested == false)
        {
            _watchDog?.Feed(); // apply health check
            List<MessageWithBag<TEvent>> payload = new();

            // Se isso não funcionar... deve escalar pra thread principal.
            var receivedMessages = await receiver.ReceiveMessagesAsync(
                   maxMessages: 128,
                   maxWaitTime: TimeSpan.FromMinutes(5),
                   cancellationToken);

            if (receivedMessages == null) continue;

            try
            {
                payload = receivedMessages
                .Select(x => new MessageWithBag<TEvent>(
                    x.Body.ToObjectFromJson<TEvent>(new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    new AzureServiceBusBatchMessageBag<TEvent>(x, receiver)))
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
    }

    public void Dispose() => DisposeAsync().GetAwaiter();
}
