using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.UpdatePet;

namespace PawsAndHearts.PetManagement.IntegrationTests.Pets;

public class UpdatePetTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, UpdatePetCommand> _sut;

    public UpdatePetTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, UpdatePetCommand>>();
    }

    [Fact]
    public async Task Update_Pet()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (volunteerId, petId) = await SeedPet(cancellationToken);

        var command = _fixture.CreateUpdatePetCommand(volunteerId, petId);
        
        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var pet = await _readDbContext.Pets
            .FirstOrDefaultAsync(p => p.Id == petId, cancellationToken);

        pet.Should().NotBeNull();
        pet?.Name.Should().Be(command.Name);
    }
}