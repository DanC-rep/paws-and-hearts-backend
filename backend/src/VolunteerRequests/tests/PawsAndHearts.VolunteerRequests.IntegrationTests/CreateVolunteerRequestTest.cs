using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Application.UseCases.CreateVolunteerRequest;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class CreateVolunteerRequestTest : VolunteerRequestsTestsBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerRequestCommand> _sut;

    public CreateVolunteerRequestTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerRequestCommand>>();
    }

    [Fact]
    public async Task Add_Volunteer_Request_To_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        var command = _fixture.CreateAddVolunteerRequestCommand();

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var volunteerRequest = await _readDbContext.VolunteerRequests
            .FirstOrDefaultAsync(v => v.Id == result.Value, cancellationToken);

        volunteerRequest.Should().NotBeNull();
    }
}