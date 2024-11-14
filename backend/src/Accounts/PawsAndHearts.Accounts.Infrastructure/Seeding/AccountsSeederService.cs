using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.IdentityManagers;
using PawsAndHearts.Accounts.Infrastructure.Options;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Options;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Infrastructure.Seeding;

public class AccountsSeederService
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IAccountManager _accountManager;
    private readonly UserManager<User> _userManager;
    private readonly PermissionManager _permissionManager;
    private readonly RolePermissionManager _rolePermissionManager;
    private readonly AdminOptions _adminOptions;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountsSeederService> _logger;
    
    public AccountsSeederService(
        RoleManager<Role> roleManager,
        IAccountManager accountManager,
        UserManager<User> userManager,
        PermissionManager permissionManager,
        RolePermissionManager rolePermissionManager,
        IOptions<AdminOptions> adminOptions,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork,
        ILogger<AccountsSeederService> logger)
    {
        _roleManager = roleManager;
        _accountManager = accountManager;
        _userManager = userManager;
        _permissionManager = permissionManager;
        _rolePermissionManager = rolePermissionManager;
        _adminOptions = adminOptions.Value;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var json = await File.ReadAllTextAsync(SharedKernel.Constants.FilePaths.ACCOUNTS, cancellationToken);

        var seedData = JsonSerializer.Deserialize<RolePermissionOptions>(json)
            ?? throw new ApplicationException("Could not deserialize role permission config");

        await SeedPermissions(seedData, cancellationToken);

        await SeedRoles(seedData, cancellationToken);

        await SeedRolePermissions(seedData, cancellationToken);

        await SeedAdminAccount(cancellationToken);
    }

    private async Task SeedPermissions(
        RolePermissionOptions seedData, 
        CancellationToken cancellationToken = default)
    {
        var permissionsToAdd = seedData.Permissions
            .SelectMany(permissionGroup => permissionGroup.Value);

        await _permissionManager.AddRangeIfExists(permissionsToAdd, cancellationToken);
        
        _logger.LogInformation("Permissions added to database");
    }

    private async Task SeedRoles(
        RolePermissionOptions seedData, 
        CancellationToken cancellationToken = default)
    {
        foreach (var role in seedData.Roles.Keys)
        {
            var existingRole = await _roleManager.FindByNameAsync(role);

            if (existingRole is null)
                await _roleManager.CreateAsync(new Role { Name = role });
        }
        
        _logger.LogInformation("Roles added to database");
    }

    private async Task SeedRolePermissions(
        RolePermissionOptions seedData,
        CancellationToken cancellationToken = default)
    {
        foreach (var roleName in seedData.Roles.Keys)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
                throw new ApplicationException($"Role {roleName} not found");
            
            await _rolePermissionManager
                .AddRangeIfNotExists(role.Id, seedData.Roles[roleName], cancellationToken);
        }
        
        _logger.LogInformation("Role permissions added to database");
    }

    private async Task SeedAdminAccount(CancellationToken cancellationToken = default)
    {
        var adminExists = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == _adminOptions.Email, cancellationToken);

        if (adminExists is not null)
            return;
            
        
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == Domain.Constants.ADMIN, cancellationToken);

        if (role is null)
            throw new ApplicationException($"Role {Domain.Constants.ADMIN} not found");

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var fullName = FullName.Create(
                _adminOptions.UserName, 
                _adminOptions.UserName, 
                _adminOptions.UserName).Value;

            var userResult = User.CreateAdminAccount(
                _adminOptions.UserName,
                _adminOptions.Email,
                fullName,
                role);

            if (userResult.IsFailure)
                throw new ApplicationException(userResult.Error.Message);

            await _userManager.CreateAsync(userResult.Value, _adminOptions.Password);

            var adminAccount = new AdminAccount(userResult.Value);
            
            await _accountManager.AddAdminAccount(adminAccount, cancellationToken);
            
            await _unitOfWork.SaveChanges(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation("Admin account added to database");
            
        }
        catch (Exception ex)
        {
            _logger.LogError("Creating admin was failed");
            
            await transaction.RollbackAsync(cancellationToken);

            throw new ApplicationException(ex.Message);
        }
    }
}