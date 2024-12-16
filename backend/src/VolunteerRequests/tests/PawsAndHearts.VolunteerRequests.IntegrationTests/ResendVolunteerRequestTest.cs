using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Application.UseCases.ResendVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Domain.Enums;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class ResendVolunteerRequestTest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<ResendVolunteerRequestCommand> _sut;

    public ResendVolunteerRequestTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<ResendVolunteerRequestCommand>>();
    }

    [Fact]
    public async Task Resend_Volunteer_Request()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, userId) = await SendVolunteerRequestForRevision(cancellationToken);

        var command = new ResendVolunteerRequestCommand(userId, requestId);
        
        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        var volunteerRequest = await _readDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == requestId, cancellationToken);

        volunteerRequest.Should().NotBeNull();
        volunteerRequest?.Status.Should().Be(VolunteerRequestStatus.Submitted.ToString());
    }
}