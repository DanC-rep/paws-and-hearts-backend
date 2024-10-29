using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;

namespace PawsAndHearts.Accounts.Infrastructure.IdentityManagers;

public class RolePermissionManager
{
    private readonly AccountsWriteDbContext _accountsWriteDbContext;
    
    public RolePermissionManager(AccountsWriteDbContext accountsWriteDbContext)
    {
        _accountsWriteDbContext = accountsWriteDbContext;
    }

    public async Task AddRangeIfNotExists(
        Guid roleId, 
        IEnumerable<string> permissions,
        CancellationToken cancellationToken = default)
    {
        foreach (var permissionCode in permissions)
        {
            var permission = await _accountsWriteDbContext.Permissions
                .FirstOrDefaultAsync(p => p.Code == permissionCode, cancellationToken);
            
            if (permission is null)
                throw new ApplicationException($"Permission code {permissionCode} is not found");
            
            var rolePermissionExists = await _accountsWriteDbContext.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id, cancellationToken);
            
            if (rolePermissionExists)
                continue;

            await _accountsWriteDbContext.RolePermissions.AddAsync(
                new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id
                }, cancellationToken);
        }

        await _accountsWriteDbContext.SaveChangesAsync(cancellationToken);
    }
}