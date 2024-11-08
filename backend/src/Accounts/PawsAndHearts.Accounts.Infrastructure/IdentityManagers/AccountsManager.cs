using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Infrastructure.IdentityManagers;

public class AccountsManager(AccountsWriteDbContext accountsWriteDbContext) : IAccountManager
{
    public async Task AddParticipantAccount(
        ParticipantAccount participantAccount, 
        CancellationToken cancellationToken = default)
    {
        await accountsWriteDbContext.ParticipantAccounts.AddAsync(participantAccount, cancellationToken);
    }

    public async Task AddAdminAccount(AdminAccount adminAccount, CancellationToken cancellationToken = default)
    {
        await accountsWriteDbContext.AdminAccounts.AddAsync(adminAccount, cancellationToken);
    }
    
    public async Task AddVolunteerAccount(
        VolunteerAccount volunteerAccount, 
        CancellationToken cancellationToken = default)
    {
        await accountsWriteDbContext.VolunteerAccounts.AddAsync(volunteerAccount, cancellationToken);
    }

    public async Task<Result<VolunteerAccount, Error>> GetVolunteerAccountByUserId(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var volunteerAccount = await accountsWriteDbContext.VolunteerAccounts
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken);

        if (volunteerAccount is null)
            return Errors.General.NotFound(userId);

        return volunteerAccount;
    }
}