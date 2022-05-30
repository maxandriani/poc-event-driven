using Poc.EventDriven.Services.Requests.Abstractions;

namespace Poc.EventDriven.Services.Requests;

public class PagedRequest : IPagedRequest
{
    public int? Take { get; set; } = 0;
    public int? Page { get; set; } = 0;
}