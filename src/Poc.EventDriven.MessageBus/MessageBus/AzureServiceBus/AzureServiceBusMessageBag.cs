using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

using Poc.EventDriven.MessageBus.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusMessageBag<TEvent> : IMessageBag
    where TEvent : class, IMessageBusEvent
{
    private readonly ProcessMessageEventArgs _messageArgs;

    public AzureServiceBusMessageBag(ProcessMessageEventArgs messageArgs)
    {
        _messageArgs = messageArgs;
        Attempts = _messageArgs.Message.DeliveryCount;
    }

    public int Attempts { get; private set; } = 0;

    public bool Resolved { get; private set; } = false;

    public Task AbortAsync(Exception? reason)
    {
        CheckResolvedMessage();
        Resolved = true;
        return _messageArgs.DeadLetterMessageAsync(_messageArgs.Message, reason?.Message);
    }

    public Task CompleteAsync()
    {
        CheckResolvedMessage();
        Resolved = true;
        return _messageArgs.CompleteMessageAsync(_messageArgs.Message);
    }

    public Task ReScheduleAsync()
    {
        CheckResolvedMessage();
        Resolved = true;
        return _messageArgs.AbandonMessageAsync(_messageArgs.Message, propertiesToModify: new Dictionary<string, object>
        {
            { "Attempts", Attempts++ }
        });
    }

    private void CheckResolvedMessage()
    {
        if (Resolved) throw new InvalidOperationException($"You can not ReSchedule a Resolved Message {_messageArgs.Message.To}");
    }
}
