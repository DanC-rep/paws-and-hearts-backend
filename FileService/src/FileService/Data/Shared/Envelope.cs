namespace FileService.Data.Shared;

public record ResponseError(string? ErrorCode, string? ErrorMessage, string? InvalidField);

public record Envelope
{
    private Envelope(object? result, IEnumerable<ResponseError>? errors)
    {
        Result = result;
        ResponseErrors = errors?.ToList();
        TimeGenerated = DateTime.Now;
    }

    public object? Result { get; }
    
    public IReadOnlyList<ResponseError>? ResponseErrors { get; }
    
    public DateTime TimeGenerated { get; }

    public static Envelope Ok(object? result) =>
        new Envelope(result, null);

    public static Envelope Error(ErrorList errors)
    {
        var responseErrors = errors
            .Select(e => new ResponseError(e.Code, e.Message, e.InvalidField)).ToList();

        return new Envelope(null, responseErrors);
    }
}