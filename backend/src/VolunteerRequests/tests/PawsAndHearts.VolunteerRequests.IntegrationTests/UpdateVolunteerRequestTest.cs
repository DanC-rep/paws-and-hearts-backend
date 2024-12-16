using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Application.UseCases.UpdateVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Domain.Enums;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class UpdateVolunteerRequestTest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<UpdateVolunteerRequestCommand> _sut;

    public UpdateVolunteerRequestTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<UpdateVolunteerRequestCommand>>();
    }

    [Fact]
    public async Task Update_Volunteer_Request()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, userId) = await SendVolunteerRequestForRevision(cancellationToken);

        var command = _fixture.CreateUpdateVolunteerRequestCommand(requestId, userId);
        
        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        var volunteerRequest = await _readDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == requestId, cancellationToken);

        volunteerRequest.Should().NotBeNull();
        volunteerRequest?.Status.Should().Be(VolunteerRequestStatus.Revision.ToString());
    }
}