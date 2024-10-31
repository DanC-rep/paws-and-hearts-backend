using CSharpFunctionalExtensions;

namespace PawsAndHearts.SharedKernel.ValueObjects.Ids;

public class MessageId : ValueObject
{
    private MessageId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }

    public static MessageId NewId() => new(Guid.NewGuid());

    public static MessageId Empty() => new(Guid.Empty);

    public static MessageId Create(Guid id) => new(id);

    public static implicit operator MessageId(Guid id) => new(id);

    public static implicit operator Guid(MessageId breedId)
    {
        ArgumentNullException.ThrowIfNull(breedId);

        return breedId.Value;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}