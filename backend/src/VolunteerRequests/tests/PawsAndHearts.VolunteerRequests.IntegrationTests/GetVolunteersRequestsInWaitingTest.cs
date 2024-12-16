using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteersRequestsInWaitingWithPagination;
using PawsAndHearts.VolunteerRequests.Contracts.Dtos;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class GetVolunteersRequestsInWaitingTest : VolunteerRequestsTestsBase
{
    private readonly IQueryHandler<PagedList<VolunteerRequestDto>, 
        GetVolunteersRequestsInWaitingWithPaginationQuery> _sut;

    public GetVolunteersRequestsInWaitingTest(IntegrationTestsWebFactory factory): base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<IQueryHandler<PagedList<VolunteerRequestDto>,
            GetVolunteersRequestsInWaitingWithPaginationQuery>>();
    }

    [Fact]
    public async Task Get_Volunteers_Requests_In_Waiting()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, userId) = await SeedVolunteerRequest(cancellationToken);

        var query = new GetVolunteersRequestsInWaitingWithPaginationQuery(null, null, 1, 1);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].Id.Should().Be(requestId);
    }
}