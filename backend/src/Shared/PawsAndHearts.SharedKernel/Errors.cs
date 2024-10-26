namespace PawsAndHearts.SharedKernel;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null) =>
            Error.Validation("value.is.invalid", $"{name ?? "value"} is invalid");
        
        public static Error ValueIsRequired(string? name = null) =>
            Error.Validation("length.is.invalid", $"invalid {name ?? "value"} length");

        public static Error NotFound(Guid? id = null, string? name = null)
        {
            var forId = id == null ? "" : $" for Id '{id}'";
            return Error.NotFound("record.not.found", $"{name ?? "record"} not found{forId}");
        }

        public static Error AlreadyExists(string name, string key, string? value = null)
        {
            var withValue = value == null ? "" : $" = {value}";
            
            return Error.Conflict("record.already.exists", $"{name} already exists with {key + withValue}");
        }
        
        public static Error AlreadyUsed(Guid id) =>
            Error.Conflict("value.already.used", $"{id} is already used");
        
        public static Error Null(string? name = null)
        {
            var label = name ?? "value";
            return Error.Null("Null.entity", $"{label} is null");
        }
    }

    public static class Files
    {
        public static Error InvalidSize() =>
            Error.Validation("size.is.invalid", "File size is invalid");

        public static Error InvalidExtension() =>
            Error.Validation("extension.is.invalid", "File extension is invalid");
    }

    public static class Accounts
    {
        public static Error InvalidCredentials()
        {
            return Error.Validation("credentials.is.invalid", "Your credentials is invalid");
        }

        public static Error InvalidRole()
        {
            return Error.Failure("invalid.role", "Invalid role");
        }
    }

    public static class Tokens
    {
        public static Error TokenExpired()
        {
            return Error.Validation("token.expired", "Token has expired");
        }
        
        public static Error InvalidToken()
        {
            return Error.Validation("token.is.invalid", "Token is invalid");
        }
    }
}