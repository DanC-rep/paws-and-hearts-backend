using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Application.Models;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.Accounts.Infrastructure.DbContexts;
using PawsAndHearts.Core.Models;
using PawsAndHearts.Core.Options;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Infrastructure.Providers;

public class JwtTokenProvider : ITokenProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly RefreshSessionOptions _refreshSessionOptions;
    private readonly AccountsWriteDbContext _accountsWriteDbContext;
    private readonly IPermissionManager _permissionManager;
    
    public JwtTokenProvider(
        IOptions<JwtOptions> jwtOptions,
        IOptions<RefreshSessionOptions> refreshSessionOptions,
        AccountsWriteDbContext accountsWriteDbContext,
        IPermissionManager permissionManager)
    {
        _jwtOptions = jwtOptions.Value;
        _refreshSessionOptions = refreshSessionOptions.Value;
        _accountsWriteDbContext = accountsWriteDbContext;
        _permissionManager = permissionManager;
    }
    
    public async Task<JwtTokenResult> GenerateAccessToken(User user, CancellationToken cancellationToken = default)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roleClaims = user.Roles
            .Select(r => new Claim(CustomClaims.Role, r.Name ?? string.Empty));

        var permissions = await _permissionManager.GetPermissionsByUserId(user.Id, cancellationToken);
        var permissionsClaims = permissions.Value.Select(p => new Claim(CustomClaims.Permission, p));

        var jti = Guid.NewGuid();

        Claim[] claims =
        [
            new Claim(CustomClaims.Id, user.Id.ToString()),
            new Claim(CustomClaims.Email, user.Email ?? ""),
            new Claim(CustomClaims.Jti, jti.ToString())
        ];

        claims = claims
            .Concat(roleClaims)
            .Concat(permissionsClaims)
            .ToArray();
        
        var jwtToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_jwtOptions.ExpiredMinutesTime)),
            signingCredentials: signingCredentials,
            claims: claims);
        
        var stringToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new JwtTokenResult(stringToken, jti);
    }

    public async Task<Guid> GenerateRefreshToken(User user,
        Guid accessTokenJti,
        CancellationToken cancellationToken = default)
    {
        var refreshSession = new RefreshSession
        {
            User = user,
            CreationDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(int.Parse(_refreshSessionOptions.ExpiredDaysTime)),
            Jti = accessTokenJti,
            RefreshToken = Guid.NewGuid()
        };

        _accountsWriteDbContext.RefreshSessions.Add(refreshSession);
        await _accountsWriteDbContext.SaveChangesAsync(cancellationToken);

        return refreshSession.RefreshToken;
    }

    public async Task<Result<IReadOnlyList<Claim>, Error>> GetUserClaims(
        string jwtToken, 
        CancellationToken cancellationToken = default)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        var validationParameters = TokenValidationParametersFactory.CreateWithoutLifeTime(_jwtOptions);

        var validationResult =  await jwtHandler.ValidateTokenAsync(jwtToken, validationParameters);

        if (!validationResult.IsValid)
            return Errors.Tokens.InvalidToken();

        return validationResult.ClaimsIdentity.Claims.ToList();
    }
}