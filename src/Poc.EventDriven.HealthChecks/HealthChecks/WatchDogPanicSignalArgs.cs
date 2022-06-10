using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.HealthChecks;

public class WatchDogPanicSignalArgs
{
    public string Reason { get; set; } = string.Empty;
    public HealthStatus Status { get; set; } = HealthStatus.Unhealthy;
}
