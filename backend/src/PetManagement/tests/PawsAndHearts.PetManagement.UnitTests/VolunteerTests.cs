using FluentAssertions;
using PawsAndHearts.PetManagement.Domain.Entities;
using PawsAndHearts.PetManagement.Domain.Enums;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.PetManagement.UnitTests;

public class VolunteerTests
{
    [Fact]
    public void Add_Pet_Return_Success_Result()
    {
        // arrange
        var phoneNumber = PhoneNumber.Create("89202068434").Value;
        var petIdentity = new PetIdentity(SpeciesId.Empty(), Guid.Empty);
        var color = Color.Create("123").Value;
        var address = Address.Create("123", "123", "123", "213").Value;
        var petMetrics = PetMetrics.Create(123, 213).Value;
        var birthDate = BirthDate.Create(DateTime.Now).Value;
        var creationDate = CreationDate.Create(DateTime.Now).Value;
        var petId = PetId.NewId();
        
        var requisites = new ValueObjectList<Requisite>(
            [Requisite.Create("123", "123").Value]);
        
        var volunteer = CreateVolunteerWithPets(0);

        var pet = new Pet(
            petId,
            "123",
            "123",
            petIdentity,
            color,
            "norm",
            address,
            petMetrics,
            phoneNumber,
            true,
            birthDate,
            true,
            HelpStatus.FoundHome,
            creationDate,
            requisites);
        
        // act
        var result = volunteer.AddPet(pet);

        var addedPetResult = volunteer.GetPetById(petId);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        addedPetResult.IsSuccess.Should().BeTrue();
        
        addedPetResult.Value.Id.Should().Be(pet.Id);
        addedPetResult.Value.Position.Should().Be(Position.First);
    }

    [Fact]
    public void Move_Pet_Should_Not_Move_When_Pet_Already_At_New_Position()
    {
        // arrange
        const int petsCount = 5;

        var volunteer = CreateVolunteerWithPets(petsCount);

        var secondPosition = Position.Create(2).Value;

        var firstPet = volunteer.Pets[0];
        var secondPet = volunteer.Pets[1];
        var thirdPet = volunteer.Pets[2];
        var fourthPet = volunteer.Pets[3];
        var fifthPet = volunteer.Pets[4];
        
        // act
        var result = volunteer.MovePet(secondPet, secondPosition);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Should().Be(Position.Create(1).Value);
        secondPet.Position.Should().Be(Position.Create(2).Value);
        thirdPet.Position.Should().Be(Position.Create(3).Value);
        fourthPet.Position.Should().Be(Position.Create(4).Value);
        fifthPet.Position.Should().Be(Position.Create(5).Value);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Forward_When_New_Position_Is_Lower()
    {
        // arrange 
        const int petsCount = 5;

        var volunteer = CreateVolunteerWithPets(petsCount);

        var secondPosition = Position.Create(2).Value;

        var firstPet = volunteer.Pets[0];
        var secondPet = volunteer.Pets[1];
        var thirdPet = volunteer.Pets[2];
        var fourthPet = volunteer.Pets[3];
        var fifthPet = volunteer.Pets[4];
        
        // act
        var result = volunteer.MovePet(fourthPet, secondPosition);

        // assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Should().Be(Position.Create(1).Value);
        secondPet.Position.Should().Be(Position.Create(3).Value);
        thirdPet.Position.Should().Be(Position.Create(4).Value);
        fourthPet.Position.Should().Be(Position.Create(2).Value);
        fifthPet.Position.Should().Be(Position.Create(5).Value);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Back_When_New_Position_Is_Greater()
    {
        // arrange  
        const int petsCount = 5;

        var volunteer = CreateVolunteerWithPets(petsCount);

        var fourthPosition = Position.Create(4).Value;

        var firstPet = volunteer.Pets[0];
        var secondPet = volunteer.Pets[1];
        var thirdPet = volunteer.Pets[2];
        var fourthPet = volunteer.Pets[3];
        var fifthPet = volunteer.Pets[4];

        // act
        var result = volunteer.MovePet(secondPet, fourthPosition);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Should().Be(Position.Create(1).Value);
        secondPet.Position.Should().Be(Position.Create(4).Value);
        thirdPet.Position.Should().Be(Position.Create(2).Value);
        fourthPet.Position.Should().Be(Position.Create(3).Value);
        fifthPet.Position.Should().Be(Position.Create(5).Value);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Forward_When_New_Position_Is_First()
    {
        // arrange
        const int petsCount = 5;

        var volunteer = CreateVolunteerWithPets(5);

        var firstPosition = Position.Create(1).Value;
        
        var firstPet = volunteer.Pets[0];
        var secondPet = volunteer.Pets[1];
        var thirdPet = volunteer.Pets[2];
        var fourthPet = volunteer.Pets[3];
        var fifthPet = volunteer.Pets[4];
        
        // act
        var result = volunteer.MovePet(fifthPet, firstPosition);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Should().Be(Position.Create(2).Value);
        secondPet.Position.Should().Be(Position.Create(3).Value);
        thirdPet.Position.Should().Be(Position.Create(4).Value);
        fourthPet.Position.Should().Be(Position.Create(5).Value);
        fifthPet.Position.Should().Be(Position.Create(1).Value);
    }

    [Fact]
    public void Move_Pet_Should_Move_Other_Pets_Back_New_Position_Is_Last()
    {
        // arrange
        const int petsCount = 5;

        var volunteer = CreateVolunteerWithPets(5);

        var fifthPosition = Position.Create(5).Value;
        
        var firstPet = volunteer.Pets[0];
        var secondPet = volunteer.Pets[1];
        var thirdPet = volunteer.Pets[2];
        var fourthPet = volunteer.Pets[3];
        var fifthPet = volunteer.Pets[4];
        
        // act
        var result = volunteer.MovePet(firstPet, fifthPosition);
        
        // assert
        result.IsSuccess.Should().BeTrue();
        firstPet.Position.Should().Be(Position.Create(5).Value);
        secondPet.Position.Should().Be(Position.Create(1).Value);
        thirdPet.Position.Should().Be(Position.Create(2).Value);
        fourthPet.Position.Should().Be(Position.Create(3).Value);
        fifthPet.Position.Should().Be(Position.Create(4).Value);
    }
    
    private Volunteer CreateVolunteerWithPets(int petsCount)
    {
        var petId = PetId.NewId();
        var fullName = FullName.Create("123", "123", "123").Value;
        var experience = Experience.Create(1).Value;
        var phoneNumber = PhoneNumber.Create("89785578809").Value;
        var petIdentity = new PetIdentity(SpeciesId.Empty(), Guid.Empty);
        var color = Color.Create("123").Value;
        var address = Address.Create("123", "123", "123", "213").Value;
        var petMetrics = PetMetrics.Create(123, 213).Value;
        var birthDate = BirthDate.Create(DateTime.Now).Value;
        var creationDate = CreationDate.Create(DateTime.Now).Value;

        var requisites = new ValueObjectList<Requisite>(
            [Requisite.Create("123", "123").Value]);
        
        var volunteer = new Volunteer(
            VolunteerId.NewId(),
            fullName,
            experience,
            phoneNumber);

        for (int i = 0; i < petsCount; i++)
        {
            var pet = new Pet(
                petId,
                "123",
                "123",
                petIdentity,
                color,
                "norm",
                address,
                petMetrics,
                phoneNumber,
                true,
                birthDate,
                true,
                HelpStatus.FoundHome,
                creationDate,
                requisites);

            volunteer.AddPet(pet);
        }

        return volunteer;
    }
}