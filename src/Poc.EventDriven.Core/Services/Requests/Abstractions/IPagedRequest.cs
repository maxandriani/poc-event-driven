namespace Poc.EventDriven.Services.Requests.Abstractions;

public interface IPagedRequest
{
    int? Take { get; set; }
    int? Page { get; set; }
}
