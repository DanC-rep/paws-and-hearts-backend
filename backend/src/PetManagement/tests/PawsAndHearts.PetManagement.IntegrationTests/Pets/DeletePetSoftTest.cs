using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.UseCases.DeletePetSoft;

namespace PawsAndHearts.PetManagement.IntegrationTests.Pets;

public class DeletePetSoftTest : PetManagementTestsBase
{
    private readonly ICommandHandler<Guid, DeletePetSoftCommand> _sut;

    public DeletePetSoftTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeletePetSoftCommand>>();
    }

    [Fact]
    public async Task Soft_Delete_Pet()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (volunteerId, petId) = await SeedPet(cancellationToken);

        var command = new DeletePetSoftCommand(volunteerId, petId);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var pet = await _readDbContext.Pets.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == petId, cancellationToken);

        pet.Should().NotBeNull();
        pet?.IsDeleted.Should().BeTrue();
    }
}