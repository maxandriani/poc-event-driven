namespace Poc.EventDriven.Services.Requests;

public class GetByKeyRequest<TKey>
{
    public TKey? Id { get; set; }
}

public class GetByKeyRequest : GetByKeyRequest<int> { };