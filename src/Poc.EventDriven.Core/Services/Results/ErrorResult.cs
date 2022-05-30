using System.Runtime.Serialization;

namespace Poc.EventDriven.Services.Results;

[Serializable]
public class ErrorResult : ISerializable
{
    public List<ErrorMessage> Errors { get; private set; } = new();

    public ErrorResult(ErrorMessage error)
    {
        Errors.Add(error);
    }

    public ErrorResult(List<ErrorMessage> errors)
    {
        Errors = errors;
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null) throw new ArgumentNullException(nameof(info));

        info.AddValue(nameof(Errors), Errors);
    }
}

[Serializable]
public class ErrorMessage : ISerializable
{
    public string Message { get; private set; } = string.Empty;
    public string? Field { get; private set; }

    public ErrorMessage(string message, string? field)
    {
        Message = message;
        Field = field;
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null) throw new ArgumentNullException(nameof(info));

        info.AddValue(nameof(Message), Message);
        info.AddValue(nameof(Field), Field);
    }
}