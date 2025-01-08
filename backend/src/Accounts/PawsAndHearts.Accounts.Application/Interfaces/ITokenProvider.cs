using System.Security.Claims;
using CSharpFunctionalExtensions;
using PawsAndHearts.Accounts.Application.Models;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Application.Interfaces;

public interface ITokenProvider
{
   Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken = default);
   Task<Guid> GenerateRefreshToken(
      User user, Guid accessTokenJti, CancellationToken cancellationToken = default);

   Task<Result<IReadOnlyList<Claim>, Error>> GetUserClaims(
      string jwtToken, CancellationToken cancellationToken = default);
}