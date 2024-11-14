using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Discussions.Domain.ValueObjects;

public class MessageText : ValueObject
{
    public string Value { get; }

    private MessageText()
    {
        
    }

    private MessageText(string value)
    {
        Value = value;
    }

    public static Result<MessageText, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > Constants.MAX_DESCRIPTION_LENGTH)
            return Errors.General.ValueIsInvalid("rejection comment");

        return new MessageText(value);
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}