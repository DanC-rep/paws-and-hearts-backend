using CSharpFunctionalExtensions;
using PawsAndHearts.PetManagement.Domain.Enums;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.PetManagement.Domain.Entities;

public class Pet : Entity<PetId>, ISoftDeletable
{
    private Pet(PetId id) : base(id)
    {
    }

    public Pet(
        PetId id,
        string name,
        string description,
        PetIdentity petIdentity,
        Color color, 
        string healthInfo,
        Address address,
        PetMetrics petMetrics,
        PhoneNumber phoneNumber,
        bool isNeutered,
        BirthDate birthDate,
        bool isVaccinated,
        HelpStatus helpStatus,
        CreationDate creationDate,
        ValueObjectList<Requisite> requisites) : base(id)
    {
        Name = name;
        Description = description;
        PetIdentity = petIdentity;
        Color = color;
        HealthInfo = healthInfo;
        Address = address;
        PetMetrics = petMetrics;
        PhoneNumber = phoneNumber;
        IsNeutered = isNeutered;
        BirthDate = birthDate;
        IsVaccinated = isVaccinated;
        HelpStatus = helpStatus;
        CreationDate = creationDate;
        Requisites = requisites;
    }

    public string Name { get; private set; } = default!;

    public string Description { get; private set; } = default!;
    
    public Position Position { get; private set; }
    
    public PetIdentity PetIdentity { get; private set; }

    public Color Color { get; private set; }

    public string HealthInfo { get; private set; } = default!;

    public Address Address { get; private set; }

    public PetMetrics PetMetrics { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }
    
    public bool IsNeutered { get; private set; }
    
    public BirthDate BirthDate { get; private set; }
    
    public bool IsVaccinated { get; private set; }
    
    public HelpStatus HelpStatus { get; private set; }
    
    public CreationDate CreationDate { get; private set; }
    
    public VolunteerId VolunteerId { get; private set; }

    public IReadOnlyList<Requisite> Requisites { get; private set; }

    public IReadOnlyList<PetPhoto>? PetPhotos { get; private set; }
    
    public bool IsDeleted { get; private set; }
    
    public DateTime? DeletionDate { get; private set; }

    public void SetPosition(Position position) =>
        Position = position;

    internal void AddPhotos(IEnumerable<PetPhoto> petPhotos)
    {
        PetPhotos = petPhotos.ToList();
    }

    internal void UpdateStatus(HelpStatus helpStatus)
    {
        HelpStatus = helpStatus;
    }

    internal UnitResult<Error> MoveForward()
    {
        var newPosition = Position.Forward();

        if (newPosition.IsFailure)
            return newPosition.Error;

        Position = newPosition.Value;

        return Result.Success<Error>();
    }
    
    internal UnitResult<Error> MoveBack()
    {
        var newPosition = Position.Back();

        if (newPosition.IsFailure)
            return newPosition.Error;

        Position = newPosition.Value;

        return Result.Success<Error>();
    }

    internal void Move(Position newPosition)
    {
        Position = newPosition;
    }

    internal void UpdateInfo(Pet updatedPet)
    {
        Name = updatedPet.Name;
        Description = updatedPet.Description;
        PetIdentity = updatedPet.PetIdentity;
        Color = updatedPet.Color;
        HealthInfo = updatedPet.HealthInfo;
        Address = updatedPet.Address;
        PetMetrics = updatedPet.PetMetrics;
        PhoneNumber = updatedPet.PhoneNumber;
        IsNeutered = updatedPet.IsNeutered;
        BirthDate = updatedPet.BirthDate;
        IsVaccinated = updatedPet.IsVaccinated;
        HelpStatus = updatedPet.HelpStatus;
        CreationDate = updatedPet.CreationDate;
        Requisites = updatedPet.Requisites;
    }

    internal void DeletePhotos()
    {
        PetPhotos = [];
    }

    internal UnitResult<Error> UpdateMainPhoto(PetPhoto updatedPhoto)
    {
        var fileExists = PetPhotos?
            .Where(p => p.FileId == updatedPhoto.FileId).FirstOrDefault();

        if (fileExists is null)
            return Errors.General.NotFound();
        
        PetPhotos = PetPhotos?
            .Select(photo => PetPhoto.Create(photo.FileId, photo.FileId == updatedPhoto.FileId).Value)
            .OrderByDescending(photo => photo.IsMain)
            .ToList();

        return Result.Success<Error>();
    }

    public void Delete()
    {
        if (IsDeleted)
            return;
        
        IsDeleted = true;
        DeletionDate = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
            return;
        
        IsDeleted = false;
        DeletionDate = null;
    }
}