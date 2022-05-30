using Poc.EventDriven.MessageBus.Abstractions;

namespace Poc.EventDriven.DwNf.Events;

public class NfConsolidacaoEvent : IMessageBusEvent
{
    public string Cliente { get; set; } = string.Empty;
    public string ChaveNf { get; set; } = string.Empty;
    public string BloblAddress { get; set; } = string.Empty;
}
