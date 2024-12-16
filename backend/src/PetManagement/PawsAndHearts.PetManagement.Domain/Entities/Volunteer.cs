using CSharpFunctionalExtensions;
using PawsAndHearts.PetManagement.Domain.Enums;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.PetManagement.Domain.Entities;

public class Volunteer : SoftDeletableEntity<VolunteerId>
{
    private Volunteer(VolunteerId id) : base(id)
    {
    }
    
    public Volunteer(
        VolunteerId id, 
        FullName fullName, 
        Experience experience,
        PhoneNumber phoneNumber) : base(id)
    {
        FullName = fullName;
        Experience = experience;
        PhoneNumber = phoneNumber;
    }

    public FullName FullName { get; private set; }
    
    public Experience Experience { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    private readonly List<Pet> _pets = [];

    public IReadOnlyList<Pet> Pets => _pets;

    public int GetPetsFoundHome() => _pets.Count(p => p.HelpStatus == HelpStatus.FoundHome);

    public int GetPetsLookingForHome() => _pets.Count(p => p.HelpStatus == HelpStatus.LookingForHome);

    public int GetPetsBeingTreated() => _pets.Count(p => p.HelpStatus == HelpStatus.NeedForHelp);

    public void UpdateMainInfo(FullName fullName, PhoneNumber phoneNumber, Experience experience)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Experience = experience;
    }

    public override void Delete()
    {
        base.Delete();

        foreach (var pet in _pets)
            pet.Delete();
    }

    public override void Restore()
    {
        base.Restore();

        foreach (var pet in _pets)
            pet.Restore();
    }

    public UnitResult<Error> AddPet(Pet pet)
    {
        var positionResult = Position.Create(_pets.Count + 1);

        if (positionResult.IsFailure)
            return positionResult.Error;
        
        pet.SetPosition(positionResult.Value);
        
        _pets.Add(pet);

        return Result.Success<Error>();
    }

    public void UpdatePet(Pet updatedPet)
    {
        var pet = GetPetById(updatedPet.Id).Value;

        pet.UpdateInfo(updatedPet);
    }

    public void DeletePet(Pet pet)
    {
        _pets.Remove(pet);
    }

    public void UpdatePetStatus(Pet pet, HelpStatus helpStatus)
    {
        pet.UpdateStatus(helpStatus);
    }

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        var currentPosition = pet.Position;

        if (currentPosition == newPosition || _pets.Count == 1)
            return Result.Success<Error>();

        var adjustedPosition = AdjustNewPositionIfOutOfRange(newPosition);

        if (adjustedPosition.IsFailure)
            return adjustedPosition.Error;

        newPosition = adjustedPosition.Value;

        var moveResult = MovePetsBetweenPosition(newPosition, currentPosition);

        if (moveResult.IsFailure)
            return moveResult.Error;

        pet.Move(newPosition);

        return Result.Success<Error>();
    }

    private Result<Position, Error> AdjustNewPositionIfOutOfRange(Position newPosition)
    {
        if (newPosition.Value <= _pets.Count)
            return newPosition;
        
        var lastPosition = Position.Create(_pets.Count);
        
        if (lastPosition.IsFailure)
            return lastPosition.Error;

        return lastPosition.Value;
    }

    private UnitResult<Error> MovePetsBetweenPosition(Position newPosition, Position currentPosition)
    {
        if (newPosition.Value < currentPosition.Value)
        {
            var petsToMove = _pets.Where(p => p.Position.Value >= newPosition.Value 
                                              && p.Position.Value < currentPosition.Value);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveForward();

                if (result.IsFailure)
                    return result.Error;
            }
        }
        else if (newPosition.Value > currentPosition.Value)
        {
            var petsToMove = _pets.Where(p => p.Position.Value > currentPosition.Value
                                              && p.Position.Value <= newPosition.Value);

            foreach (var petToMove in petsToMove)
            {
                var result = petToMove.MoveBack();

                if (result.IsFailure)
                    return result.Error;
            }
        }

        return Result.Success<Error>();
    }

    public Result<Pet, Error> GetPetById(PetId petId)
    {
        var pet = _pets.FirstOrDefault(p => p.Id == petId);

        if (pet is null)
            return Errors.General.NotFound(petId);

        return pet;
    }


    public Result<Guid, Error> UpdatePetMainPhoto(Pet pet, PetPhoto petPhoto)
    {
        var result = pet.UpdateMainPhoto(petPhoto);

        if (result.IsFailure)
            return result.Error;

        return petPhoto.FileId;
    }

    public void AddPetPhotos(Pet pet, IEnumerable<PetPhoto> photos)
    {
        pet.AddPhotos(photos);
    }


    public void DeletePetPhotos(Pet pet)
    {
        pet.DeletePhotos();
    }

    public void DeleteExpiredPets(int expiredDays)
    {
        _pets.RemoveAll(p => p.DeletionDate != null
                             && DateTime.UtcNow >= p.DeletionDate.Value.AddDays(expiredDays));
    }
}