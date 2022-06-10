using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.HealthChecks.Abstractions;

public interface IWatchDogMonitor
{
    public HealthStatus GetHealthStatus();
    public IEnumerable<WatchDogPanicSignalArgs> PanicEvents(Func<WatchDogPanicSignalArgs, bool>? predicate = null);
    public string Describe();
    public void Observe(IWatchDog watchdog);
    public void Flush(IWatchDog watchDog);
}
