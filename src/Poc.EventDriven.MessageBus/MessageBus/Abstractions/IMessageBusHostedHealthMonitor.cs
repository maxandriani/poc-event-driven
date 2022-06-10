using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.MessageBus.Abstractions;

public interface IMessageBusHostedHealthMonitor
{
    Guid RegisterWhitness(Task task, TimeSpan maxHealthDelay);
    bool IsHealth();
}
