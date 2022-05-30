using Azure.Messaging.ServiceBus;

using Poc.EventDriven.MessageBus.Abstractions;

namespace Poc.EventDriven.MessageBus.AzureServiceBus;

sealed internal class AzureServiceBusBatchMessageBag<TEvent> : IMessageBag
    where TEvent : class, IMessageBusEvent
{
    private readonly ServiceBusReceivedMessage _message;
    private readonly ServiceBusReceiver _receiver;

    public AzureServiceBusBatchMessageBag(
        ServiceBusReceivedMessage message,
        ServiceBusReceiver receiver)
    {
        _message = message;
        _receiver = receiver;
        Attempts = message.DeliveryCount;
    }

    public int Attempts { get; private set; } = 0;

    public bool Resolved { get; private set; } = false;

    public Task AbortAsync(Exception? reason)
    {
        CheckResolvedMessage();
        Resolved = true;
        return _receiver.DeadLetterMessageAsync(_message, reason?.Message);
    }

    public Task CompleteAsync()
    {
        CheckResolvedMessage();
        Resolved = true;
        return _receiver.CompleteMessageAsync(_message);
    }

    public Task ReScheduleAsync()
    {
        CheckResolvedMessage();
        Resolved = true;
        return _receiver.AbandonMessageAsync(_message, propertiesToModify: new Dictionary<string, object>()
        {
            { "Attempts", Attempts++ },
        });
    }

    private void CheckResolvedMessage()
    {
        if (Resolved) throw new InvalidOperationException($"You can not ReSchedule a Resolved Message {_message.To}");
    }
}
