using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.UseCases.Login;
using PawsAndHearts.Accounts.Contracts.Responses;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.IntegrationTests;

public class LoginUserTest : AccountsTestsBase
{
    private readonly ICommandHandler<LoginResponse, LoginUserCommand> _sut;

    public LoginUserTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginResponse, LoginUserCommand>>();
    }

    [Fact]
    public async Task Login_User()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles(cancellationToken);

        var response = await SeedUser(cancellationToken);

        var command = new LoginUserCommand(response.Email, response.Password);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }
}