using PawsAndHearts.SharedKernel.Enums;

namespace PawsAndHearts.SharedKernel;

public abstract class CustomException : Exception
{
    public string Code { get; }
    
    public ErrorType Type { get; }

    public string? InvalidField { get; }

    public CustomException(Error error) 
        : base(error.Message)
    {
        Code = error.Code;
        Type = error.Type;
        InvalidField = error.InvalidField;
    }
}

public class CanNotCreateRecordException : CustomException
{
    public CanNotCreateRecordException(Error error) : base(error) { }
}

public class AccountBannedException : CustomException
{
    public AccountBannedException(Error error) : base(error) { }
}

public class DeleteRecordException : CustomException
{
    public DeleteRecordException(Error error) : base(error) { }
}