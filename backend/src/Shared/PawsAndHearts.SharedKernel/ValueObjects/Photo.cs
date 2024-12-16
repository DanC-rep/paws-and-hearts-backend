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
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return FileId;
    }
}