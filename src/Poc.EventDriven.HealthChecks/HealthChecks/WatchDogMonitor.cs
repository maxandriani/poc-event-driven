using Microsoft.Extensions.Diagnostics.HealthChecks;
using Poc.EventDriven.HealthChecks.Abstractions;

namespace Poc.EventDriven.HealthChecks;

internal class WatchDogMonitor : IWatchDogMonitor
{
    private readonly Dictionary<IWatchDog, WatchDogPanicSignalArgs> _panicEvents = new();
    private HealthStatus? _cachedStatus = null;

    public string Describe()
    {
        return string.Join(",\n", _panicEvents.Values.OrderBy(q => q.Status).Select(v => v.Reason));
    }

    public void Flush(IWatchDog watchDog)
    {
        if (_panicEvents.ContainsKey(watchDog) == true)
        {
            _panicEvents.Remove(watchDog);
            _cachedStatus = null;
        }
    }

    public HealthStatus GetHealthStatus()
    {
        if (_cachedStatus == null)
        {
            _cachedStatus = _panicEvents.Count > 0 ? _panicEvents.Values.Min(q => q.Status) : HealthStatus.Healthy;
        }

        return _cachedStatus!.Value;
    }

    public void Observe(IWatchDog watchdog)
    {
        watchdog.PanicSignal += (s, e) =>
        {
            if (e.Status == HealthStatus.Unhealthy || e.Status == HealthStatus.Healthy)
            {
                _panicEvents[watchdog] = e;
            }
            else
            {
                Flush(watchdog);
            }
        };

        watchdog.FlushSignal += (s, e) => Flush(watchdog);
    }

    public IEnumerable<WatchDogPanicSignalArgs> PanicEvents(Func<WatchDogPanicSignalArgs, bool>? predicate = null)
    {
        return _panicEvents.Values.Where(predicate ?? (_ => true));
    }
}
