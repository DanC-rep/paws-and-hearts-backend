namespace PawsAndHearts.SharedKernel;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    
    DateTime? DeletionDate { get; }

    void Delete();

    void Restore();
}