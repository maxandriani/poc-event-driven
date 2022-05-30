namespace Poc.EventDriven.MessageBus.Abstractions;

public interface IMessageBag
{
    int Attempts { get; }
    bool Resolved { get; }
    Task CompleteAsync(); // Aknowledge
    Task ReScheduleAsync(); // Re-Delivery
    Task AbortAsync(Exception? reason = default); // Dead Letter
}
