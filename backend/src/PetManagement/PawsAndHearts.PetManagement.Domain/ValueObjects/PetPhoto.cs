using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.PetManagement.Domain.ValueObjects;

public class PetPhoto : ValueObject
{
    private static string[] PERMITTED_FILE_TYPES = { "image/jpg", "image/jpeg", "image/png", "image/gif" };

    private static string[] PERMITTED_EXTENSIONS = { "jpg", "jpeg", "png", "gif" };

    private static long MAX_FILE_SIZE = 5242880;
    
    private PetPhoto(Guid fileId, bool isMain)
    {
        FileId = fileId;
        IsMain = isMain;
    }
    
    public Guid FileId { get; }
    
    public bool IsMain { get; }

    public static Result<PetPhoto, Error> Create(Guid fileId, bool isMain)
    {
        return new PetPhoto(fileId, isMain);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return FileId;
        yield return IsMain;
    }
}