using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Application.UseCases.TakeRequestForSubmit;
using PawsAndHearts.VolunteerRequests.Domain.Enums;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class TakeRequestForSubmitTest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<TakeRequestForSubmitCommand> _sut;

    public TakeRequestForSubmitTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<TakeRequestForSubmitCommand>>();
    }

    [Fact]
    public async Task Take_Request_For_Submit()
    {
        // arrange
        _factory.SetupDiscussionsContractMock();
        
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, userId) = await SeedVolunteerRequest(cancellationToken);

        var command = new TakeRequestForSubmitCommand(requestId, Guid.NewGuid());

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