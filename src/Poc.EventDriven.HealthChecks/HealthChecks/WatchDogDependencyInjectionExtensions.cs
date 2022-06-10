using Microsoft.Extensions.DependencyInjection;
using Poc.EventDriven.HealthChecks.Abstractions;

namespace Poc.EventDriven.HealthChecks;

public static class WatchDogDependencyInjectionExtensions
{
    public static IServiceCollection AddWatchDogServices(this IServiceCollection services)
    {
        if (services.Any(q => q.ServiceType.Equals(typeof(IWatchDogMonitor)))) return services;
        services.AddSingleton<IWatchDogMonitor, WatchDogMonitor>();
        services.AddSingleton<IWatchDogFactory, WatchDogFactory>();
        return services;
    }

    public static IHealthChecksBuilder AddWatchDogCheck(this IHealthChecksBuilder builder)
    {
        builder.AddCheck<WatchDogHealthCheck>("long-running-background-services");
        return builder;
    }
}
