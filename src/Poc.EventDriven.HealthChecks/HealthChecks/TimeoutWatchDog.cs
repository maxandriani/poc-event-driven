using Microsoft.Extensions.Diagnostics.HealthChecks;

using Poc.EventDriven.HealthChecks.Abstractions;

namespace Poc.EventDriven.HealthChecks;

public class TimeoutWatchDog : IWatchDog
{
    private readonly System.Timers.Timer _timer;
    public event EventHandler<WatchDogPanicSignalArgs>? PanicSignal;
    public event EventHandler? FlushSignal;

    public TimeoutWatchDog(TimeSpan timeout, int tolerance, string reason)
    {
        _timer = new(timeout.TotalMilliseconds);
        _timer.Elapsed += (s, e) => PanicSignal?.Invoke(this, new WatchDogPanicSignalArgs
        {
            Reason = reason,
            Status = (tolerance-- <= 0) ? HealthStatus.Unhealthy : HealthStatus.Degraded
        });
        _timer.Start();
    }

    public TimeoutWatchDog(TimeSpan timeout, string reason) : this(timeout, 3, reason) { }

    public void Dispose()
    {
        FlushSignal?.Invoke(this, EventArgs.Empty);
    }

    public void Feed()
    {
        _timer.Stop();
        _timer.Start(); // Yes... this is a reset();
    }
}
