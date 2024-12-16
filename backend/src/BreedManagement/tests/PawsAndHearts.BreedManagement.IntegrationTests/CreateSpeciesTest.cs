using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.UseCases.CreateSpecies;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class CreateSpeciesTest : BreedManagementTestsBase
{
    private readonly ICommandHandler<Guid, CreateSpeciesCommand> _sut;

    public CreateSpeciesTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<Guid, CreateSpeciesCommand>>();
    }

    [Fact]
    public async Task Add_Species_To_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var command = _fixture.CreateAddSpeciesCommand();

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var species = _readDbContext.Species
            .FirstOrDefaultAsync(s => s.Id == result.Value, cancellationToken);
        
        species.Should().NotBeNull();
    }
}