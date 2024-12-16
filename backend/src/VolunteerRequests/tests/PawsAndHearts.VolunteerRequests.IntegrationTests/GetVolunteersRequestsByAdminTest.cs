using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteersRequestsByAdminWithPagination;
using PawsAndHearts.VolunteerRequests.Contracts.Dtos;

namespace PawsAndHearts.VolunteerRequests.IntegrationTests;

public class GetVolunteersRequestsByAdminTest : VolunteerRequestsTestsBase
{
    private readonly IQueryHandler<PagedList<VolunteerRequestDto>, 
            GetVolunteersRequestsByAdminWithPaginationQuery> _sut;

    public GetVolunteersRequestsByAdminTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<IQueryHandler<PagedList<VolunteerRequestDto>,
            GetVolunteersRequestsByAdminWithPaginationQuery>>();
    }

    [Fact]
    public async Task Get_Volunteers_Requests_By_Admin()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var (requestId, adminId) = await TakeVolunteerRequestForSubmit(cancellationToken);

        var query = new GetVolunteersRequestsByAdminWithPaginationQuery(null, null, 1, 1, null, adminId);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].Id.Should().Be(requestId);
    }
}