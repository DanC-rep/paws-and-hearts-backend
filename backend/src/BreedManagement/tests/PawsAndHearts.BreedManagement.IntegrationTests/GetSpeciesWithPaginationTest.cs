using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.BreedManagement.Application.Queries.GetSpeciesWithPagination;
using PawsAndHearts.BreedManagement.Contracts.Dtos;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Models;

namespace PawsAndHearts.BreedManagement.IntegrationTests;

public class GetSpeciesWithPaginationTest : BreedManagementTestsBase
{
    private readonly IQueryHandler<PagedList<SpeciesDto>, GetSpeciesWithPaginationQuery> _sut;

    public GetSpeciesWithPaginationTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<SpeciesDto>, GetSpeciesWithPaginationQuery>>();
    }

    [Fact]
    public async Task Get_Species_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var species = await SeedSpecies(cancellationToken);

        var query = new GetSpeciesWithPaginationQuery(null, 1, 1);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].Id.Should().Be(species.Id);
    }
}