using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.PetManagement.Application.Queries.GetPetsWIthPagination;
using PawsAndHearts.PetManagement.Contracts.Dtos;
using PawsAndHearts.PetManagement.Contracts.Responses;

namespace PawsAndHearts.PetManagement.IntegrationTests.Pets;

public class GetPetsWithPaginationTest : PetManagementTestsBase
{
    private readonly IQueryHandler<PagedList<PetResponse>, GetPetsWithPaginationQuery> _sut;

    public GetPetsWithPaginationTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<PetResponse>, GetPetsWithPaginationQuery>>();
    }

    [Fact]
    public async Task Get_Volunteers_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (volunteerId, petId) = await SeedPet(cancellationToken);

        var query = new GetPetsWithPaginationQuery(
            null, null, 1, 1, null, null, null, null, null, null, null,
            null, null, 0, null, null, null, null, null, null);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].Id.Should().Be(petId);
    }
}