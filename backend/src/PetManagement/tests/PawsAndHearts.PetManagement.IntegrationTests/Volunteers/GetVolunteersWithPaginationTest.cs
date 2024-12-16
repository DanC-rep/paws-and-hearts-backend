using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.PetManagement.Application.Queries.GetVolunteersWithPagination;
using PawsAndHearts.PetManagement.Contracts.Dtos;

namespace PawsAndHearts.PetManagement.IntegrationTests.Volunteers;

public class GetVolunteersWithPaginationTest : PetManagementTestsBase
{
    private readonly IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery> _sut;

    public GetVolunteersWithPaginationTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PagedList<VolunteerDto>, GetVolunteersWithPaginationQuery>>();
    }

    [Fact]
    public async Task Get_Volunteers_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteerId = await SeedVolunteer(cancellationToken);

        var query = new GetVolunteersWithPaginationQuery(null, null, 1, 1);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].Id.Should().Be(volunteerId);
    }
}