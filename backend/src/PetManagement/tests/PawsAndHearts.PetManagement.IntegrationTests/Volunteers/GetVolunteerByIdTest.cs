using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.PetManagement.Application.Queries.GetVolunteerById;
using PawsAndHearts.PetManagement.Contracts.Dtos;

namespace PawsAndHearts.PetManagement.IntegrationTests.Volunteers;

public class GetVolunteerByIdTest : PetManagementTestsBase
{
    private readonly IQueryHandlerWithResult<VolunteerDto, GetVolunteerByIdQuery> _sut;
    
    public GetVolunteerByIdTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandlerWithResult<VolunteerDto, GetVolunteerByIdQuery>>();
    }

    [Fact]
    public async Task Get_Volunteer_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteerId = await SeedVolunteer(cancellationToken);

        var query = new GetVolunteerByIdQuery(volunteerId);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(volunteerId);
    }
}