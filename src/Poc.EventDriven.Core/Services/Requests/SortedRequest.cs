using Poc.EventDriven.Services.Requests.Abstractions;

namespace Poc.EventDriven.Services.Requests;

public class SortedRequest : ISortedRequest
{
    public string? Sorting { get; set; }
}