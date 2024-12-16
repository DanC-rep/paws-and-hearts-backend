using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Application.UseCases.SendVolunteerRequestForRevision;
using PawsAndHearts.VolunteerRequests.Domain.Enums;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class SendVolunteerRequestForRevisionTest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<SendVolunteerRequestForRevisionCommand> _sut;

    public SendVolunteerRequestForRevisionTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<SendVolunteerRequestForRevisionCommand>>();
    }

    [Fact]
    public async Task Send_Volunteer_Request_For_Revision()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, adminId) = await TakeVolunteerRequestForSubmit(cancellationToken);
        
        var command = new SendVolunteerRequestForRevisionCommand(adminId, requestId, "test");

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