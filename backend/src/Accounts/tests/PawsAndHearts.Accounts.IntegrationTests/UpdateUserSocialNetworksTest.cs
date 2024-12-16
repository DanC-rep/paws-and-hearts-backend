using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.UseCases.UpdateUserSocialNetworks;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.IntegrationTests;

public class UpdateUserSocialNetworksTest : AccountsTestsBase
{
    private readonly ICommandHandler<UpdateUserSocialNetworksCommand> _sut;

    public UpdateUserSocialNetworksTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<UpdateUserSocialNetworksCommand>>();
    }

    [Fact]
    public async Task Update_User_Social_Networks()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles(cancellationToken);

        var response = await SeedUser(cancellationToken);

        var command = _fixture.CreateUpdateUserSocialNetworksCommand(response.Id);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }
}