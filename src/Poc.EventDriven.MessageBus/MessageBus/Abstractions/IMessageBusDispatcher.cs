namespace Poc.EventDriven.MessageBus.Abstractions
{
    public interface IMessageBusDispatcher
    {
        Task DispatchAsync<TEvent>(
            TEvent eventMessage,
            CancellationToken cancellationToken = default)
                where TEvent : class, IMessageBusEvent;
    }
}
