using Microsoft.Extensions.Hosting;

namespace Poc.EventDriven.MessageBus.Abstractions;

public interface IMessageBusManager: IHostedService, IDisposable, IAsyncDisposable
{
}
