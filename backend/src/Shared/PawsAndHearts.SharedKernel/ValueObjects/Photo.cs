using CSharpFunctionalExtensions;

namespace PawsAndHearts.SharedKernel.ValueObjects;

public class Photo : ValueObject
{
    private static string[] PERMITTED_FILE_TYPES = { "image/jpg", "image/jpeg", "image/png", "image/gif" };

    private static string[] PERMITTED_EXTENSIONS = { "jpg", "jpeg", "png", "gif" };

    private static long MAX_FILE_SIZE = 5242880;

    public Photo(Guid fileId)
    {
        FileId = fileId;
    }
    
    public Guid FileId { get; }
    
    public static UnitResult<Error> Validate(
        string fileName,
        string contentType,
        long? size = null)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return Errors.General.ValueIsInvalid("file name");
        
        var extension = Path.GetExtension(fileName).Substring(1);

        if (PERMITTED_EXTENSIONS.All(x => x != extension))
            return Errors.General.ValueIsInvalid("file extension");

        if (PERMITTED_FILE_TYPES.All(x => x != contentType))
            return Errors.General.ValueIsInvalid("content type");

        if (size > MAX_FILE_SIZE)
            return Errors.Files.InvalidSize();

        return Result.Success<Error>();
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return FileId;
    }
}