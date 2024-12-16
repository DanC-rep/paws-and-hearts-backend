using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.UseCases.DeleteSpecies;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class DeleteSpeciesTest : BreedManagementTestsBase
{
    private readonly ICommandHandler<Guid, DeleteSpeciesCommand> _sut;

    public DeleteSpeciesTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, DeleteSpeciesCommand>>();
    }

    [Fact]
    public async Task Remove_Species_From_Database()
    {
        // arrange
        _factory.SetupPetManagementContractMock();
        
        var cancellationToken = new CancellationTokenSource().Token;
        
        var species = await SeedSpecies(cancellationToken);

        var command = new DeleteSpeciesCommand(species.Id);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var speciesList = await _readDbContext.Species.ToListAsync(cancellationToken);

        speciesList.Should().BeEmpty();
    }
}