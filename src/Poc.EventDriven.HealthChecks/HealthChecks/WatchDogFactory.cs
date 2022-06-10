using Poc.EventDriven.HealthChecks.Abstractions;

namespace Poc.EventDriven.HealthChecks;

public class WatchDogFactory : IWatchDogFactory
{
    private readonly IWatchDogMonitor _monitor;

    public WatchDogFactory(IWatchDogMonitor monitor)
    {
        _monitor = monitor;
    }

    public IWatchDog CreatePredicateWatchDog(Func<bool> predicate, string reason)
    {
        var wd = new PredicateWatchDog(predicate, reason);
        _monitor.Observe(wd);
        return wd;
    }

    public IWatchDog CreatePredicateWatchDog(Func<bool> predicate, TimeSpan timeout, string reason)
    {
        var wd = new PredicateWatchDog(predicate, timeout, 3, reason);
        _monitor.Observe(wd);
        return wd;
    }

    public IWatchDog CreateTimeoutWatchDog(TimeSpan timeout, string reason)
    {
        var wd = new TimeoutWatchDog(timeout, reason);
        _monitor.Observe(wd);
        return wd;
    }
}
