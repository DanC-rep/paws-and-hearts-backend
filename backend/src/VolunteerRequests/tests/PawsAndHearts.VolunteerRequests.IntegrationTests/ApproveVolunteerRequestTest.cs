using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Application.UseCases.ApproveVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Domain.Enums;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class ApproveVolunteerRequestTest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<ApproveVolunteerRequestCommand> _sut;

    public ApproveVolunteerRequestTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<ApproveVolunteerRequestCommand>>();
    }

    [Fact]
    public async Task Approve_Volunteer_Request()
    {
        // arrange
        _factory.SetupAccountsContractMock();
        
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, adminId) = await TakeVolunteerRequestForSubmit(cancellationToken);

        var command = new ApproveVolunteerRequestCommand(requestId, Guid.NewGuid());

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        var volunteerRequest = await _readDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == requestId, cancellationToken);

        volunteerRequest.Should().NotBeNull();
        volunteerRequest?.Status.Should().Be(VolunteerRequestStatus.Approved.ToString());
    }
}