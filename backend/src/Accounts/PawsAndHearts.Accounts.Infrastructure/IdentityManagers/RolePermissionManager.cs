using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Domain;

namespace PawsAndHearts.Accounts.Infrastructure.IdentityManagers;

public class RolePermissionManager
{
    private readonly AccountsDbContext _accountsDbContext;
    
    public RolePermissionManager(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task AddRangeIfNotExists(
        Guid roleId, 
        IEnumerable<string> permissions,
        CancellationToken cancellationToken = default)
    {
        foreach (var permissionCode in permissions)
        {
            var permission = await _accountsDbContext.Permissions
                .FirstOrDefaultAsync(p => p.Code == permissionCode, cancellationToken);
            
            if (permission is null)
                throw new ApplicationException($"Permission code {permissionCode} is not found");
            
            var rolePermissionExists = await _accountsDbContext.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id, cancellationToken);
            
            if (rolePermissionExists)
                continue;

            await _accountsDbContext.RolePermissions.AddAsync(
                new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id
                }, cancellationToken);
        }

        await _accountsDbContext.SaveChangesAsync(cancellationToken);
    }
}