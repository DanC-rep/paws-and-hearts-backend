using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.Queries.GetBreedsBySpecies;
using PawsAndHearts.BreedManagement.Contracts.Dtos;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class GetBreedsBySpeciesTest : BreedManagementTestsBase
{
    private readonly IQueryHandlerWithResult<IEnumerable<BreedDto>, GetBreedsBySpeciesQuery> _sut;

    public GetBreedsBySpeciesTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandlerWithResult<IEnumerable<BreedDto>, GetBreedsBySpeciesQuery>>();
    }

    [Fact]
    public async Task Get_Breeds_By_Species()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var species = await SeedSpeciesWithBreed(cancellationToken);

        var command = new GetBreedsBySpeciesQuery(species.Id, null);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}