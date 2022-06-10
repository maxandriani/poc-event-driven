using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.HealthChecks.Abstractions;

public interface IWatchDog : IDisposable
{
    public event EventHandler<WatchDogPanicSignalArgs> PanicSignal;
    public event EventHandler FlushSignal;
    public void Feed();
}
