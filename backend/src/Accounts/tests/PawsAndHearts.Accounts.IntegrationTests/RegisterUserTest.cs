using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.UseCases.Register;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.IntegrationTests;

public class RegisterUserTest : AccountsTestsBase
{
    private readonly ICommandHandler<RegisterUserCommand> _sut;

    public RegisterUserTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand>>();
    }

    [Fact]
    public async Task Add_User_To_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles(cancellationToken);
        
        var command = _fixture.CreateRegisterUserCommand();

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var user = await _readDbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == command.UserName, cancellationToken);

        user.Should().NotBeNull();
    }
}