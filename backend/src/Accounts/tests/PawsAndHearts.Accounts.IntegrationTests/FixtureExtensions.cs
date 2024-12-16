using AutoFixture;
using PawsAndHearts.Accounts.Application.UseCases.CreateVolunteerAccount;
using PawsAndHearts.Accounts.Application.UseCases.Register;
using PawsAndHearts.Accounts.Application.UseCases.UpdateUserSocialNetworks;

namespace PawsAndHearts.Accounts.IntegrationTests;

public static class FixtureExtensions
{
    public static RegisterUserCommand CreateRegisterUserCommand(this Fixture fixture)
    {
        return fixture.Build<RegisterUserCommand>()
            .With(c => c.Email, "abc123@gmail.com")
            .With(c => c.Password, "U897623hgt!?")
            .Create();
    }

    public static UpdateUserSocialNetworksCommand CreateUpdateUserSocialNetworksCommand(
        this Fixture fixture, Guid userId)
    {
        return fixture.Build<UpdateUserSocialNetworksCommand>()
            .With(c => c.UserId, userId)
            .Create();
    }

    public static CreateVolunteerAccountCommand CreateAddVolunteerAccountCommand(
        this Fixture fixture, Guid userId)
    {
        return fixture.Build<CreateVolunteerAccountCommand>()
            .With(c => c.UserId, userId)
            .With(c => c.Experience, 5)
            .Create();
    }
}