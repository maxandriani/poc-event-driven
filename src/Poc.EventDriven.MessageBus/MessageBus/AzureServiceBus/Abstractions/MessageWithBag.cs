using Poc.EventDriven.MessageBus.Abstractions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.EventDriven.MessageBus.AzureServiceBus.Abstractions;

public record struct MessageWithBag<TEvent> (TEvent Body, IMessageBag Bag);
