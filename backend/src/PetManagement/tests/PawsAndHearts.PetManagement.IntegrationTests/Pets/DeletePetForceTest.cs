using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.DeletePetForce;

namespace PawsAndHearts.PetManagement.IntegrationTests.Pets;

public class DeletePetForceTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, DeletePetForceCommand> _sut;

    public DeletePetForceTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeletePetForceCommand>>();
    }

    [Fact]
    public async Task Remove_Pet_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (volunteerId, petId) = await SeedPet(cancellationToken);

        var command = new DeletePetForceCommand(volunteerId, petId);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var pets = _readDbContext.Pets.ToList();
        pets.Should().BeEmpty();
    }
}