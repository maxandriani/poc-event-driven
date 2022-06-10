using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.HealthChecks.Abstractions;

public interface IWatchDogFactory
{
    public IWatchDog CreateTimeoutWatchDog(TimeSpan timeout, string reason);
    public IWatchDog CreatePredicateWatchDog(Func<bool> predicate, string reason);
    public IWatchDog CreatePredicateWatchDog(Func<bool> predicate, TimeSpan timeout, string reason);
}
