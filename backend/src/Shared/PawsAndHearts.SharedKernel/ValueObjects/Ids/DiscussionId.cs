using CSharpFunctionalExtensions;

namespace PawsAndHearts.SharedKernel.ValueObjects.Ids;

public class DiscussionId : ValueObject
{
    private DiscussionId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }

    public static DiscussionId NewId() => new(Guid.NewGuid());

    public static DiscussionId Empty() => new(Guid.Empty);

    public static DiscussionId Create(Guid id) => new(id);

    public static implicit operator DiscussionId(Guid id) => new(id);

    public static implicit operator Guid(DiscussionId breedId)
    {
        ArgumentNullException.ThrowIfNull(breedId);

        return breedId.Value;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}