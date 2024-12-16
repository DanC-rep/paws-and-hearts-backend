using AutoFixture;
using PawsAndHearts.PetManagement.Application.UseCases.CreatePet;
using PawsAndHearts.PetManagement.Application.UseCases.CreateVolunteer;
using PawsAndHearts.PetManagement.Application.UseCases.UpdateMainInfo;
using PawsAndHearts.PetManagement.Application.UseCases.UpdatePet;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.PetManagement.IntegrationTests;

public static class FixtureExtensions
{
    public static CreateVolunteerCommand CreateAddVolunteerCommand(this IFixture fixture)
    {
        return fixture.Build<CreateVolunteerCommand>()
            .With(c => c.Experience, 5)
            .With(c => c.PhoneNumber, "89457750098")
            .Create();
    }

    public static UpdateMainInfoCommand CreateUpdateMainCommand(
        this IFixture fixture,
        Guid volunteerId)
    {
        return fixture.Build<UpdateMainInfoCommand>()
            .With(c => c.VolunteerId, volunteerId)
            .With(c => c.Experience, 5)
            .With(c => c.PhoneNumber, "89765543321")
            .Create();
    }

    public static CreatePetCommand CreateAddPetCommand(
        this IFixture fixture,
        Guid volunteerId)
    {
        return fixture.Build<CreatePetCommand>()
            .With(c => c.VolunteerId, volunteerId)
            .With(c => c.PhoneNumber, "89207790099")
            .With(c => c.HelpStatus, "NeedForHelp")
            .With(c => c.BirthDate, DateTime.UtcNow)
            .With(c => c.CreationDate, DateTime.UtcNow)
            .Create();

    }

    public static UpdatePetCommand CreateUpdatePetCommand(
        this IFixture fixture,
        Guid volunteerId,
        Guid petId)
    {
        return fixture.Build<UpdatePetCommand>()
            .With(c => c.VolunteerId, volunteerId)
            .With(c => c.PetId, petId)
            .With(c => c.PhoneNumber, "89207790099")
            .With(c => c.HelpStatus, "NeedForHelp")
            .With(c => c.BirthDate, DateTime.UtcNow)
            .With(c => c.CreationDate, DateTime.UtcNow)
            .Create();
    }
}