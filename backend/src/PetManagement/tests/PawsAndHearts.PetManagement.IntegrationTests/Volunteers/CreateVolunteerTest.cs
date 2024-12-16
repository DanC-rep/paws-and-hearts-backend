using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.CreateVolunteer;

namespace PawsAndHearts.PetManagement.IntegrationTests.Volunteers;

public class CreateVolunteerTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, CreateVolunteerCommand> _sut;

    public CreateVolunteerTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateVolunteerCommand>>();
    }

    [Fact]
    public async Task Add_Volunteer_To_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var command = _fixture.CreateAddVolunteerCommand();
        
        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var volunteer = _readDbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Id == result.Value, cancellationToken);

        volunteer.Should().NotBeNull();
    }
}