using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Application.UseCases.RejectVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Domain.Enums;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class RejectVolunteerRequestTest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<RejectVolunteerRequestCommand> _sut;

    public RejectVolunteerRequestTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<RejectVolunteerRequestCommand>>();
    }

    [Fact]
    public async Task Reject_Volunteer_Request()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, adminId) = await TakeVolunteerRequestForSubmit(cancellationToken);

        var command = new RejectVolunteerRequestCommand(requestId, adminId, "test");

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var volunteerRequest = await _readDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == requestId, cancellationToken);

        volunteerRequest.Should().NotBeNull();
        volunteerRequest?.Status.Should().Be(VolunteerRequestStatus.Rejected.ToString());
    }
}