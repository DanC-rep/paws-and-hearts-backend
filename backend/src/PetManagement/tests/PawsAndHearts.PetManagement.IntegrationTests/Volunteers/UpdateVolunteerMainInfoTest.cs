using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.UpdateMainInfo;

namespace PawsAndHearts.PetManagement.IntegrationTests.Volunteers;

public class UpdateVolunteerMainInfoTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, UpdateMainInfoCommand> _sut;

    public UpdateVolunteerMainInfoTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdateMainInfoCommand>>();
    }

    [Fact]
    public async Task Update_Volunteer_Main_Info()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        var volunteerId = await SeedVolunteer(cancellationToken);

        var command = _fixture.CreateUpdateMainCommand(volunteerId);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var volunteer = await _readDbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Id == result.Value, cancellationToken);

        volunteer.Should().NotBeNull();
        volunteer?.Experience.Should().Be(command.Experience);
        volunteer?.PhoneNumber.Should().Be(command.PhoneNumber);
    }
}