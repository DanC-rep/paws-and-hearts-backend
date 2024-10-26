using CSharpFunctionalExtensions;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Application.Interfaces;

public interface IRefreshSessionManager
{
    Task<Result<RefreshSession, Error>> GetByRefreshToken
        (Guid refreshToken, CancellationToken cancellationToken = default);

    void Delete(RefreshSession refreshSession);
}