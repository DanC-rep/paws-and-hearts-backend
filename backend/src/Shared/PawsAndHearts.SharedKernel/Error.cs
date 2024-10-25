using PawsAndHearts.SharedKernel.Enums;

namespace PawsAndHearts.SharedKernel;

public record Error
{
    public static readonly string SEPARATOR = "||";
    
    private Error(string code, string message, ErrorType type, string? invalidField = null)
    {
        Code = code;
        Message = message;
        Type = type;
        InvalidField = invalidField;
    }

    public string Code { get; }
    
    public string Message { get; }
    
    public ErrorType Type { get; }

    public string? InvalidField { get; }

    public static Error Validation(string code, string message, string? invalidField = null) =>
        new (code, message, ErrorType.Validation, invalidField);
    
    public static Error Failure(string code, string message) =>
        new (code, message, ErrorType.Failure);
    
    public static Error NotFound(string code, string message) =>
        new (code, message, ErrorType.NotFound);
    
    public static Error Conflict(string code, string message) =>
        new (code, message, ErrorType.Conflict);
    
    public static Error Null(string errorCode, string errorMessage) =>
        new(errorCode, errorMessage, ErrorType.Null);

    public string Serialize()
    {
        return string.Join(SEPARATOR, Code, Message, Type);
    }

    public static Error Deserialize(string serialized)
    {
        var parts = serialized.Split(SEPARATOR);

        if (parts.Length < 3)
        {
            throw new ArgumentException("Invalid serialized format");
        }

        if (Enum.TryParse<ErrorType>(parts[2], out var type) == false)
        {
            throw new ArgumentException("Invalid serialized format");
        }
            
        return new Error(parts[0], parts[1], type);
    }

    public ErrorList ToErrorList() =>
        new ErrorList([this]);
}