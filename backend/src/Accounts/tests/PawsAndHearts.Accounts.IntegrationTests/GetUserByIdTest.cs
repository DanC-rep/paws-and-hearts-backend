using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.Queries.GetUserById;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.IntegrationTests;

public class GetUserByIdTest : AccountsTestsBase
{
    private readonly IQueryHandlerWithResult<UserDto, GetUserByIdQuery> _sut;

    public GetUserByIdTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<IQueryHandlerWithResult<UserDto, GetUserByIdQuery>>();
    }

    [Fact]
    public async Task Get_User_From_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles(cancellationToken);

        var response = await SeedUser(cancellationToken);

        var query = new GetUserByIdQuery(response.Id);

        // act
        var result = await _sut.Handle(query, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(response.Id);
    }
}