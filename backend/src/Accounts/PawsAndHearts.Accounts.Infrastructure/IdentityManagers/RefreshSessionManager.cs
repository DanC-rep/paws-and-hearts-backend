using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Infrastructure.IdentityManagers;

public class RefreshSessionManager(AccountsWriteDbContext accountsWriteDbContext) : IRefreshSessionManager
{
    public async Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshSession = await accountsWriteDbContext.RefreshSessions
            .Include(r => r.User)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(r => r.RefreshToken == refreshToken, cancellationToken);
        
        if (refreshSession is null)
            return Errors.General.NotFound(refreshToken);

        return refreshSession;
    }

    public void Delete(RefreshSession refreshSession)
    {
        accountsWriteDbContext.RefreshSessions.Remove(refreshSession);
    }
}