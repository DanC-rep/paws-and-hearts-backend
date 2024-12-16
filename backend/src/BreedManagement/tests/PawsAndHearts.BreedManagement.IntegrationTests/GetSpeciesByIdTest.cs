using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.Queries.GetSpeciesById;
using PawsAndHearts.BreedManagement.Contracts.Dtos;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class GetSpeciesByIdTest : BreedManagementTestsBase
{
    private readonly IQueryHandlerWithResult<SpeciesDto, GetSpeciesByIdQuery> _sut;

    public GetSpeciesByIdTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<IQueryHandlerWithResult<SpeciesDto, GetSpeciesByIdQuery>>();
    }

    [Fact]
    public async Task Get_Species_By_Id()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var species = await SeedSpecies(cancellationToken);

        var query = new GetSpeciesByIdQuery(species.Id);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(species.Name);
    }
}