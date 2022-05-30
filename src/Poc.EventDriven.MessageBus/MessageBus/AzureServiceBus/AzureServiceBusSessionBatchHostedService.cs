using Azure.Messaging.ServiceBus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusSessionBatchHostedService<TEvent> : IMessageBusManager
    where TEvent : class, IMessageBusEvent
{
    private readonly ILogger<AzureServiceBusSessionBatchHostedService<TEvent>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusClient _client;
    private ServiceBusReceiver? _receiver { get; set; }
    private bool _hasStarted = false;
    private CancellationTokenSource? _stoppingTokenSource;
    private IOptions<IAzureServiceBusSettings> _options;
    private int _currentSession = 0;

    public AzureServiceBusSessionBatchHostedService(
        ILogger<AzureServiceBusSessionBatchHostedService<TEvent>> logger,
        IServiceProvider serviceProvider,
        IOptions<IAzureServiceBusSettings> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options;
        _client = new ServiceBusClient(_options.Value.ConnectionString, _options.Value.ClientOptions);
    }

    public bool HasSessionKeyControl() => _options.Value.SessionKeys?.Length > 0;

    public string GetNextSessionKey()
    {
        if (_currentSession == _options.Value.SessionKeys.Length)
            _currentSession = 0;
        else
            _currentSession++;

        return _options.Value.SessionKeys[_currentSession];
    }

    public Task<ServiceBusSessionReceiver> GetNextReceiver()
    {
        return (
            !string.IsNullOrWhiteSpace(_options.Value.QueueName),
            !string.IsNullOrWhiteSpace(_options.Value.TopicName),
            !string.IsNullOrWhiteSpace(_options.Value.SubscriptionName),
            HasSessionKeyControl()) switch
        {
            (true, false, false, true)  => _client.AcceptSessionAsync(_options.Value.QueueName, GetNextSessionKey(), _options.Value.SessionReceiverOptions),
            (true, false, false, false) => _client.AcceptNextSessionAsync(_options.Value.QueueName, _options.Value.SessionReceiverOptions),
            (false, true, true, true)   => _client.AcceptSessionAsync(_options.Value.TopicName, _options.Value.SubscriptionName, GetNextSessionKey(), _options.Value.SessionReceiverOptions),
            (false, true, true, false)  => _client.AcceptNextSessionAsync(_options.Value.TopicName, _options.Value.SubscriptionName, _options.Value.SessionReceiverOptions),
            (_, _, _, _) => throw new ArgumentException("Você precisa informar uma Queue ou um conjunto de Topic/Subscription, mas não ambos.")
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
        do
        {
            _receiver = await GetNextReceiver();
            _logger.LogTrace($"Service Receiver session started.");

            List<MessageWithBag<TEvent>> payload = new();
            
            do
            {
                try
                {
                    var receivedMessages = await _receiver.ReceiveMessagesAsync(
                        maxMessages: 1024,
                        maxWaitTime: TimeSpan.FromMinutes(5),
                        cancellationToken);

                    if (receivedMessages.Count() == 0) break; // Empty session circuit break

                    _logger.LogTrace($"New batch is comming...");

                    payload = receivedMessages
                        .Select(x => new MessageWithBag<TEvent>(
                            x.Body.ToObjectFromJson<TEvent>(new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            }),
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
                            _logger.LogError(ex.Message, ex, ex.StackTrace);
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
            } while (cancellationToken.IsCancellationRequested == false);

            await _receiver.CloseAsync();
        } while (cancellationToken.IsCancellationRequested == false);
    }

    public async ValueTask DisposeAsync()
    {
        _stoppingTokenSource?.Cancel();
        if (_receiver != null)
        {
            if (_receiver.IsClosed == false)
                await _receiver.CloseAsync();
            await _receiver.DisposeAsync();
        }
        await _client.DisposeAsync();
    }

    public void Dispose() => DisposeAsync().GetAwaiter();
}
