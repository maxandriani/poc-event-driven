using Poc.EventDriven.MessageBus.Abstractions;
using Poc.EventDriven.MessageBus.AzureServiceBus;
using Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class AzureServiceBusExtensions
{
    public static IAzureServiceBusConfigurationBuilder AddAzureServiceBusWorker<TEvent, TEventHandlerImp>(this IServiceCollection services)
        where TEventHandlerImp : class, IMessageBusEventHandler<TEvent>
        where TEvent: class, IMessageBusEvent
    {
        var configuration = new AzureServiceBusConfigurationBuilder();

        services.AddScoped<IMessageBusEventHandler<TEvent>, TEventHandlerImp>();
        services.AddHostedService(serviceProvider =>
        {
            return ActivatorUtilities
                .CreateInstance<AzureServiceBusHostedService<TEvent>>(
                    serviceProvider,
                    Options.Options.Create<IAzureServiceBusSettings>(configuration));
        });

        return configuration;
    }

    public static IAzureServiceBusConfigurationBuilder AddAzureServiceBusBatchWorker<TEvent, TEventHandlerImp>(this IServiceCollection services)
        where TEventHandlerImp : class, IMessageBusBatchEventHandler<TEvent>
        where TEvent : class, IMessageBusEvent
    {
        var configuration = new AzureServiceBusConfigurationBuilder();

        services.AddScoped<IMessageBusBatchEventHandler<TEvent>, TEventHandlerImp>();
        services.AddHostedService(serviceProvider =>
        {
            return ActivatorUtilities
                .CreateInstance<AzureServiceBusBatchHostedService<TEvent>>(
                    serviceProvider,
                    Options.Options.Create<IAzureServiceBusSettings>(configuration));
        });

        return configuration;
    }

    public static IAzureServiceBusConfigurationBuilder AddAzureServiceBusSessionBatchWorker<TEvent, TEventHandlerImp>(this IServiceCollection services)
        where TEventHandlerImp : class, IMessageBusBatchEventHandler<TEvent>
        where TEvent: class, IMessageBusEvent
    {
        var configuration = new AzureServiceBusConfigurationBuilder();

        services.AddScoped<IMessageBusBatchEventHandler<TEvent>, TEventHandlerImp>();
        services.AddHostedService(serviceProvider =>
        {
            return ActivatorUtilities
                .CreateInstance<AzureServiceBusSessionBatchHostedService<TEvent>>(
                    serviceProvider,
                    Options.Options.Create<IAzureServiceBusSettings>(configuration));
        });

        return configuration;
    }

    public static IAzureServiceBusDispatchConfigurationBuilder AddAzureServiceBusDispatcher(this IServiceCollection services, string clientName)
    {
        if (services.Any(q => q.ServiceType.Equals(typeof(IAzureServiceBusDispatchSettings))) == false)
        {
            services.AddSingleton<IAzureServiceBusDispatchSettings, ServiceBusDispatchConfigurations>();
        }

        if (services.Any(q => q.ServiceType.Equals(typeof(IMessageBusDispatcher))) == false)
        {
            services.AddSingleton<IMessageBusDispatcher, AzureServiceBusDispatcher>();
        }

        return new AzureServiceBusDispatchConfigurationBuilder(clientName);
    }
}
