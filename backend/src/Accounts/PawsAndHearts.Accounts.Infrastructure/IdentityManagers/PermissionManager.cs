using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Infrastructure.IdentityManagers;

public class PermissionManager : IPermissionManager
{
    private readonly AccountsWriteDbContext _accountsWriteDbContext;

    public PermissionManager(AccountsWriteDbContext accountsWriteDbContext)
    {
        _accountsWriteDbContext = accountsWriteDbContext;
    }

    public async Task AddRangeIfExists(
        IEnumerable<string> permissionsToAdd, 
        CancellationToken cancellationToken = default)
    {
        foreach (var permissionCode in permissionsToAdd)
        {
            var permissionExists = await _accountsWriteDbContext.Permissions
                .AnyAsync(p => p.Code == permissionCode, cancellationToken);

            if (permissionExists)
                return;

            await _accountsWriteDbContext.Permissions
                .AddAsync(new Permission { Code = permissionCode }, cancellationToken);
        }

        await _accountsWriteDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<IEnumerable<string>, Error>> GetPermissionsByUserId(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var permissions = await _accountsWriteDbContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .ToListAsync(cancellationToken);

        return permissions.ToList();
    }
}