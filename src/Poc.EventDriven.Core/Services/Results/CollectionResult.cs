using System.Runtime.Serialization;

namespace Poc.EventDriven.Services.Results;

[Serializable]
public class CollectionResult<TType> : ISerializable
    where TType : class
{
    public CollectionResult(IEnumerable<TType> data, int totalCount)
    {
        Items = data;
        TotalCount = totalCount;
    }

    public IEnumerable<TType> Items { get; protected set; }
    public int TotalCount { get; protected set; }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null) throw new ArgumentNullException(nameof(info));

        info.AddValue(nameof(Items), Items);
        info.AddValue(nameof(TotalCount), TotalCount);
    }
}