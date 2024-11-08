using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

public class RejectionComment : ValueObject
{
    public string Value { get; }

    private RejectionComment()
    {
        
    }

    private RejectionComment(string value)
    {
        Value = value;
    }

    public static Result<RejectionComment, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_DESCRIPTION_LENGTH)
            return Errors.General.ValueIsInvalid("rejection comment");

        return new RejectionComment(value);
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}