using Microsoft.Extensions.Diagnostics.HealthChecks;

using Poc.EventDriven.HealthChecks.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.HealthChecks;

public class PredicateWatchDog : IWatchDog
{
    public event EventHandler<WatchDogPanicSignalArgs>? PanicSignal;
    public event EventHandler? FlushSignal;
    private readonly Func<bool> _predicate;
    private readonly System.Timers.Timer _timer;

    public PredicateWatchDog(Func<bool> predicate, TimeSpan interval, int tolerance, string reason)
    {
        _predicate = predicate;
        _timer = new(interval.TotalMilliseconds);
        _timer.Elapsed += (s, e) =>
        {
            if (_predicate() == false)
            {
                PanicSignal?.Invoke(this, new WatchDogPanicSignalArgs
                {
                    Reason = reason,
                    Status = (tolerance-- <= 0) ? HealthStatus.Unhealthy : HealthStatus.Degraded
                });
            }
        };
        _timer.AutoReset = true;
        _timer.Start();
    }

    public PredicateWatchDog(Func<bool> predicate, string reason) : this(predicate, TimeSpan.FromMinutes(5), 3, reason) { }

    public void Dispose()
    {
        FlushSignal?.Invoke(this, EventArgs.Empty);
    }

    public void Feed()
    {
        _timer.Stop();
        _timer.Start();
    }
}
