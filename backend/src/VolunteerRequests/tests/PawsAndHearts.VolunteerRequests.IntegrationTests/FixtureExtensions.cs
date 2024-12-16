using AutoFixture;
using PawsAndHearts.VolunteerRequests.Application.UseCases.CreateVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Application.UseCases.UpdateVolunteerRequest;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public static class FixtureExtensions
{
    public static CreateVolunteerRequestCommand CreateAddVolunteerRequestCommand(this Fixture fixture)
    {
        return fixture.Build<CreateVolunteerRequestCommand>()
            .With(c => c.Experience, 5)
            .Create();
    }

    public static UpdateVolunteerRequestCommand CreateUpdateVolunteerRequestCommand(
        this IFixture fixture,
        Guid requestId,
        Guid userId)
    {
        return fixture.Build<UpdateVolunteerRequestCommand>()
            .With(c => c.VolunteerRequestId, requestId)
            .With(c => c.UserId, userId)
            .With(c => c.Experience, 5)
            .Create();
    }
}