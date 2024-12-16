using System.Text.Json;
using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;
using PawsAndHearts.Accounts.Infrastructure.Options;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.IntegrationTests;

public class AccountsTestsBase : IClassFixture<IntegrationTestsWebFactory>, IAsyncLifetime
{
    protected readonly Fixture _fixture;
    protected readonly IntegrationTestsWebFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly AccountsWriteDbContext _writeDbContext;
    protected readonly IReadDbContext _readDbContext;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    protected AccountsTestsBase(IntegrationTestsWebFactory factory)
    {
        _factory = factory;
        _fixture = new Fixture();
        _scope = factory.Services.CreateScope();
        _writeDbContext = _scope.ServiceProvider.GetRequiredService<AccountsWriteDbContext>();
        _readDbContext = _scope.ServiceProvider.GetRequiredService<IReadDbContext>();
        _roleManager = _scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    }

    public async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
        
        _scope.Dispose();
    }

    protected async Task SeedRoles(CancellationToken cancellationToken = default)
    {
        var json = await File.ReadAllTextAsync(SharedKernel.Constants.FilePaths.ACCOUNTS, cancellationToken);

        var seedData = JsonSerializer.Deserialize<RolePermissionOptions>(json)
                       ?? throw new ApplicationException("Could not deserialize role permission config");
        
        foreach (var role in seedData.Roles.Keys)
        {
            var existingRole = await _roleManager.FindByNameAsync(role);

            if (existingRole is null)
                await _roleManager.CreateAsync(new Role { Name = role });
        }
    }

    protected record SeedUserResponse(Guid Id, string Email, string Password);

    protected async Task<SeedUserResponse> SeedUser(CancellationToken cancellationToken = default)
    {
        var fullName = FullName.Create("name", "surname", "patronymic").Value;

        var password = "ps123DRg!?";
        var email = "test123@gmai.com";
        
        var role = await _roleManager.Roles
            .FirstAsync(r => r.Name == Constants.PARTICIPANT, cancellationToken);

        var user = User.CreateParticipant(
            "UserName123",
            email,
            fullName,
            new List<SocialNetwork>(),
            role);

        var result = await _userManager.CreateAsync(user.Value, password);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return new SeedUserResponse(user.Value.Id, email, password);
    }
}