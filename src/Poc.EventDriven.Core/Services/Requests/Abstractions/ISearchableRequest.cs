namespace Poc.EventDriven.Services.Requests.Abstractions;

public interface ISearchableRequest
{
    string? Search { get; set; }
}