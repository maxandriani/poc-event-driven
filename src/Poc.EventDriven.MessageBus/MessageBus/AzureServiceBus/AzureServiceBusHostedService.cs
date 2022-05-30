using Azure.Messaging.ServiceBus;

using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusHostedService<TEvent> : IMessageBusManager
    where TEvent : class, IMessageBusEvent
{
    private readonly ILogger<AzureServiceBusHostedService<TEvent>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;
    private bool _hasStarted = false;
    private CancellationTokenSource? _stoppingTokenSource;

    public AzureServiceBusHostedService(
        ILogger<AzureServiceBusHostedService<TEvent>> logger,
        IServiceProvider serviceProvider,
        IOptions<IAzureServiceBusSettings> _options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _client = new ServiceBusClient(_options.Value.ConnectionString, _options.Value.ClientOptions);
        var (queue, topic, subscription, _) = _options.Value;

        _processor = (!string.IsNullOrWhiteSpace(queue), !string.IsNullOrWhiteSpace(topic), !string.IsNullOrWhiteSpace(subscription)) switch
        {
            (true, false, false) => _client.CreateProcessor(queue, _options.Value.ProcessorOptions),
            (false, true, true)  => _client.CreateProcessor(topic, subscription, _options.Value.ProcessorOptions),
            (_, _, _)            => throw new ArgumentException("Você precisa informar uma Queue ou um conjunto de Topic/Subscription, mas não ambos.")
        };

        ConfigureHandlers();
    }

    private void ConfigureHandlers()
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
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
        _logger.LogError($"{args.Exception.Message}", args.ErrorSource);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hasStarted) await _processor.StopProcessingAsync();
        _stoppingTokenSource?.Cancel();
        await _client.DisposeAsync();
        await _processor.DisposeAsync();
    }

    public void Dispose() => DisposeAsync().GetAwaiter();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting event.");

        if (_hasStarted) return; // Só podemos rodar uma vez...

        // Cria um novo factory filho do token original.
        _stoppingTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _hasStarted = true; 
        await _processor.StartProcessingAsync(_stoppingTokenSource.Token);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_hasStarted)
        {
            await _processor.StopProcessingAsync();
            _hasStarted = false;
        }
    }
}