using Azure.Messaging.ServiceBus;

using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Azure;
using Poc.EventDriven.HealthChecks.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusHostedService<TEvent> : IMessageBusManager
    where TEvent : class, IMessageBusEvent
{
    private readonly ILogger<AzureServiceBusHostedService<TEvent>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWatchDogFactory _watchDogFactory;
    private readonly IAzureClientFactory<ServiceBusClient> _azureServiceBusClientFactory;
    private readonly IOptions<IAzureServiceBusSettings> _options;
    private IWatchDog? _watchDog = null;
    private ServiceBusProcessor? _processor;
    private bool _hasStarted = false;
    private CancellationTokenSource? _stoppingTokenSource;

    public AzureServiceBusHostedService(
        ILogger<AzureServiceBusHostedService<TEvent>> logger,
        IServiceProvider serviceProvider,
        IWatchDogFactory watchDogFactory,
        IAzureClientFactory<ServiceBusClient> azureServiceBusClientFactory,
        IOptions<IAzureServiceBusSettings> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _watchDogFactory = watchDogFactory;
        _azureServiceBusClientFactory = azureServiceBusClientFactory;
        _options = options;
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        _watchDog?.Feed();
        var bag = new AzureServiceBusMessageBag<TEvent>(args);

        try
        {
            _logger.LogInformation(args.Message.Body.ToString());
            var body = args.Message.Body.ToObjectFromJson<TEvent>();

            if (body == null) throw new ArgumentException($"Não foi possível serializar o evento {nameof(TEvent)}");

            using IServiceScope scope = _serviceProvider.CreateScope();
            
            // var correlation = scope.ServiceProvider.GetRequiredService<ICorrelationIdContext>();

            //if (!string.IsNullOrWhiteSpace(args.Message?.CorrelationId))
            //{
            //    correlation.ChangeCurrentId(args.Message.CorrelationId);
            //}

            var handler = scope.ServiceProvider.GetRequiredService<IMessageBusEventHandler<TEvent>>();
            await handler.HandleAsync(body, bag, _stoppingTokenSource!.Token);
        }
        catch (Exception ex)
        {
            if (!bag.Resolved) await bag.AbortAsync(ex);
            _logger.LogError(ex.Message, ex, ex.Message);
        }
        finally
        {
            if (!bag.Resolved) await bag.CompleteAsync();
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _watchDog?.Feed();
        _logger.LogError($"{args.Exception.Message}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hasStarted && _processor != null) await _processor.StopProcessingAsync();
        _stoppingTokenSource?.Cancel();
        if (_processor != null)
            await _processor.DisposeAsync();
    }

    public void Dispose() => DisposeAsync().GetAwaiter();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting event.");
        _watchDog = _watchDogFactory.CreatePredicateWatchDog(() => _processor?.IsProcessing == true, TimeSpan.FromMinutes(10), $"O processamento do evento {nameof(TEvent)} não está respondendo.");

        if (_hasStarted) return; // Só podemos rodar uma vez...

        // Cria um novo factory filho do token original.
        _stoppingTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _hasStarted = true;

        var client = _azureServiceBusClientFactory.CreateClient(_options.Value.ClientName);
        var (queue, topic, subscription, _) = _options.Value;

        _processor = (!string.IsNullOrWhiteSpace(queue), !string.IsNullOrWhiteSpace(topic), !string.IsNullOrWhiteSpace(subscription)) switch
        {
            (true, false, false) => client.CreateProcessor(queue, _options.Value.ProcessorOptions),
            (false, true, true) => client.CreateProcessor(topic, subscription, _options.Value.ProcessorOptions),
            (_, _, _) => throw new ArgumentException("Você precisa informar uma Queue ou um conjunto de Topic/Subscription, mas não ambos.")
        };

        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(_stoppingTokenSource.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_hasStarted)
        {
            if (_processor != null)
                await _processor.StopProcessingAsync();
            _hasStarted = false;
        }

        _watchDog?.Dispose();
    }
}