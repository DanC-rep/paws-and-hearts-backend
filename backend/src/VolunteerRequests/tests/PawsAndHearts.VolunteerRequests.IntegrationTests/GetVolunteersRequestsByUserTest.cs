using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteerRequestsByUserWithPagination;
using PawsAndHearts.VolunteerRequests.Contracts.Dtos;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class GetVolunteersRequestsByUserTest : VolunteerRequestsTestsBase
{
    private IQueryHandler<PagedList<VolunteerRequestDto>, GetVolunteerRequestsByUserWithPaginationQuery> _sut;

    public GetVolunteersRequestsByUserTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<IQueryHandler<PagedList<VolunteerRequestDto>,
                GetVolunteerRequestsByUserWithPaginationQuery>>();
    }

    [Fact]
    public async Task Get_Volunteer_Request_By_User()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, userId) = await SeedVolunteerRequest(cancellationToken);

        var command = new GetVolunteerRequestsByUserWithPaginationQuery(null, null, 1, 1, null, userId);
        
        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].Id.Should().Be(requestId);
    }
}