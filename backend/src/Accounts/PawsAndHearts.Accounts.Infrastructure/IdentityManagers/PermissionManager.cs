using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Infrastructure.IdentityManagers;

public class PermissionManager : IPermissionManager
{
    private readonly AccountsDbContext _accountsDbContext;

    public PermissionManager(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task AddRangeIfExists(
        IEnumerable<string> permissionsToAdd, 
        CancellationToken cancellationToken = default)
    {
        foreach (var permissionCode in permissionsToAdd)
        {
            var permissionExists = await _accountsDbContext.Permissions
                .AnyAsync(p => p.Code == permissionCode, cancellationToken);

            if (permissionExists)
                return;

            await _accountsDbContext.Permissions
                .AddAsync(new Permission { Code = permissionCode }, cancellationToken);
        }

        await _accountsDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<IEnumerable<string>, Error>> GetPermissionsByUserId(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var permissions = await _accountsDbContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .ToListAsync(cancellationToken);

        return permissions.ToList();
    }
}