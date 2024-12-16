using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.UpdatePetStatus;

namespace PawsAndHearts.PetManagement.IntegrationTests.Pets;

public class UpdatePetStatusTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, UpdatePetStatusCommand> _sut;

    public UpdatePetStatusTest(IntegrationTestsWebFactory factory): base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdatePetStatusCommand>>();
    }

    [Fact]
    public async Task Update_Pet_Status()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (volunteerId, petId) = await SeedPet(cancellationToken);

        var command = new UpdatePetStatusCommand(volunteerId, petId, "LookingForHome");

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        
        var pet = await _readDbContext.Pets
            .FirstOrDefaultAsync(p => p.Id == petId, cancellationToken);
        
        pet.Should().NotBeNull();
        pet?.HelpStatus.Should().Be(command.Status);
    }
}