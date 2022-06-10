using Microsoft.Extensions.Diagnostics.HealthChecks;
using Poc.EventDriven.HealthChecks.Abstractions;

namespace Poc.EventDriven.HealthChecks;

internal class WatchDogHealthCheck : IHealthCheck
{
    private readonly IWatchDogMonitor _watchDogMonitor;

    public WatchDogHealthCheck(IWatchDogMonitor watchDogMonitor)
    {
        _watchDogMonitor = watchDogMonitor;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = (_watchDogMonitor.GetHealthStatus()) switch
        {
            HealthStatus.Healthy => HealthCheckResult.Healthy("Everythink is fine on Narnia 😍"),
            HealthStatus.Degraded => HealthCheckResult.Degraded(_watchDogMonitor.Describe()),
            HealthStatus.Unhealthy => HealthCheckResult.Unhealthy(_watchDogMonitor.Describe()),
            _ => throw new ArgumentException("Não foi possível serializer o estado das aplicações.")
        };

        return Task.FromResult(result);
    }
}
