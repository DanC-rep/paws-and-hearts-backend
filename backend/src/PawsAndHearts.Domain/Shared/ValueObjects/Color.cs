using CSharpFunctionalExtensions;

namespace PawsAndHearts.Domain.Shared.ValueObjects;

public record Color
{
    public Color(string value)
    {
        Value = value;
    }
    
    public string Value { get; } = default!;

    public static Result<Color, Error> Create(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return Errors.General.ValueIsRequired("color");

        return new Color(color);
    }
}