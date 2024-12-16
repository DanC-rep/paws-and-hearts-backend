using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.Queries.GetBreedBySpecies;
using PawsAndHearts.BreedManagement.Contracts.Dtos;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class GetBreedBySpeciesTest : BreedManagementTestsBase
{
    private readonly IQueryHandlerWithResult<BreedDto, GetBreedBySpeciesQuery> _sut;

    public GetBreedBySpeciesTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandlerWithResult<BreedDto, GetBreedBySpeciesQuery>>();
    }

    [Fact]
    public async Task Get_Breed_By_Species()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var species = await SeedSpeciesWithBreed(cancellationToken);
        var breed = species.Breeds[0];

        var query = new GetBreedBySpeciesQuery(species.Id, breed.Id);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(breed.Name);
    }
}