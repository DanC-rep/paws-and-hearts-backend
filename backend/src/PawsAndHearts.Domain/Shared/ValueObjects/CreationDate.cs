using CSharpFunctionalExtensions;

namespace PawsAndHearts.Domain.Shared.ValueObjects;

public record CreationDate
{
    private CreationDate(DateTime value)
    {
        Value = value;
    }
    
    public DateTime Value { get; }

    public static Result<CreationDate, Error> Create(DateTime creationDate)
    {
        if (creationDate > DateTime.Now)
            return Errors.General.ValueIsInvalid("creation date");
        
        return new CreationDate(creationDate);
    }
}