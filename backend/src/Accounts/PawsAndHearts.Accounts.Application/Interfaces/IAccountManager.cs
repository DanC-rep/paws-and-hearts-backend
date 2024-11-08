using CSharpFunctionalExtensions;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Application.Interfaces;

public interface IAccountManager
{
    Task AddParticipantAccount(ParticipantAccount account, CancellationToken cancellationToken = default);

    Task AddAdminAccount(AdminAccount account, CancellationToken cancellationToken = default);
    
    Task AddVolunteerAccount(VolunteerAccount account, CancellationToken cancellationToken = default);

    Task<Result<VolunteerAccount, Error>> GetVolunteerAccountByUserId(
        Guid userId, CancellationToken cancellationToken = default);
}