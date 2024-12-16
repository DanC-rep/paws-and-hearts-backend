using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.DeleteVolunteer;

namespace PawsAndHearts.PetManagement.IntegrationTests.Volunteers;

public class DeleteVolunteerTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, DeleteVolunteerCommand> _sut;

    public DeleteVolunteerTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteVolunteerCommand>>();
    }

    [Fact]
    public async Task Soft_Delete_Volunteer()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteerId = await SeedVolunteer(cancellationToken);

        var command = new DeleteVolunteerCommand(volunteerId);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        var volunteer = await _readDbContext.Volunteers.IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == volunteerId, cancellationToken);

        volunteer.Should().NotBeNull();
        volunteer?.IsDeleted.Should().BeTrue();
    }
}