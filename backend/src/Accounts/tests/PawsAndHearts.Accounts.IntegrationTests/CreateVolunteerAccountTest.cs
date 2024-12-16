using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.UseCases.CreateVolunteerAccount;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.IntegrationTests;

public class CreateVolunteerAccountTest : AccountsTestsBase
{
    private readonly ICommandHandler<CreateVolunteerAccountCommand> _sut;

    public CreateVolunteerAccountTest(IntegrationTestsWebFactory factory) : base(factory)
    {
        _sut = _scope.ServiceProvider.GetRequiredService<ICommandHandler<CreateVolunteerAccountCommand>>();
    }

    [Fact]
    public async Task Add_Volunteer_Account_To_Database()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;

        await SeedRoles(cancellationToken);

        var response = await SeedUser(cancellationToken);

        var command = _fixture.CreateAddVolunteerAccountCommand(response.Id);

        // act
        var result = await _sut.Handle(command, cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        var volunteerAccounts = await _readDbContext.VolunteerAccounts.ToListAsync(cancellationToken);

        volunteerAccounts.Should().NotBeEmpty();
    }
}