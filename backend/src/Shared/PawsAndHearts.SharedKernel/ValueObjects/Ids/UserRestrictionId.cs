using CSharpFunctionalExtensions;

namespace PawsAndHearts.SharedKernel.ValueObjects.Ids;

public class UserRestrictionId : ValueObject
{
    private UserRestrictionId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }

    public static UserRestrictionId NewId() => new(Guid.NewGuid());

    public static UserRestrictionId Empty() => new(Guid.Empty);

    public static UserRestrictionId Create(Guid id) => new(id);

    public static implicit operator UserRestrictionId(Guid id) => new(id);

    public static implicit operator Guid(UserRestrictionId userRestrictionId)
    {
        ArgumentNullException.ThrowIfNull(userRestrictionId);

        return userRestrictionId.Value;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}