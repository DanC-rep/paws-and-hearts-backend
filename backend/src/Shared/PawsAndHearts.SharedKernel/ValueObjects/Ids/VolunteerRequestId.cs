using CSharpFunctionalExtensions;

namespace PawsAndHearts.SharedKernel.ValueObjects.Ids;

public class VolunteerRequestId : ValueObject
{
    private VolunteerRequestId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }

    public static VolunteerRequestId NewId() => new(Guid.NewGuid());

    public static VolunteerRequestId Empty() => new(Guid.Empty);

    public static VolunteerRequestId Create(Guid id) => new(id);

    public static implicit operator VolunteerRequestId(Guid id) => new(id);

    public static implicit operator Guid(VolunteerRequestId volunteerRequestId)
    {
        ArgumentNullException.ThrowIfNull(volunteerRequestId);

        return volunteerRequestId.Value;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}