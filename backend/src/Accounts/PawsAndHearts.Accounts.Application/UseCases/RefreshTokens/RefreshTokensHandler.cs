using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Contracts.Responses;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Enums;
using PawsAndHearts.Core.Models;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Application.UseCases.RefreshTokens;

public class RefreshTokensHandler : ICommandHandler<LoginResponse, RefreshTokensCommand>
{
    private readonly IRefreshSessionManager _refreshSessionManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokensHandler(
        IRefreshSessionManager refreshSessionManager,
        ITokenProvider tokenProvider,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork)
    {
        _refreshSessionManager = refreshSessionManager;
        _tokenProvider = tokenProvider;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<LoginResponse, ErrorList>> Handle(
        RefreshTokensCommand command, 
        CancellationToken cancellationToken = default)
    {
        var oldRefreshSessionResult = await _refreshSessionManager
            .GetByRefreshToken(command.RefreshToken, cancellationToken);

        if (oldRefreshSessionResult.IsFailure)
            return oldRefreshSessionResult.Error.ToErrorList();

        if (oldRefreshSessionResult.Value.ExpirationDate < DateTime.UtcNow)
            return Errors.Tokens.TokenExpired().ToErrorList();

        var userClaimsResult = await _tokenProvider.GetUserClaims(command.AccessToken, cancellationToken);

        if (userClaimsResult.IsFailure)
            return userClaimsResult.Error.ToErrorList();

        var userIdString = userClaimsResult.Value.FirstOrDefault(c => c.Type == CustomClaims.Id)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Errors.General.Null().ToErrorList();

        if (oldRefreshSessionResult.Value.UserId != userId)
            return Errors.Tokens.InvalidToken().ToErrorList();
        
        var userJtiString = userClaimsResult.Value.FirstOrDefault(c => c.Type == CustomClaims.Jti)?.Value;

        if (!Guid.TryParse(userJtiString, out var userJti))
            return Errors.General.Null().ToErrorList();

        if (oldRefreshSessionResult.Value.Jti != userJti)
            return Errors.Tokens.InvalidToken().ToErrorList();

        _refreshSessionManager.Delete(oldRefreshSessionResult.Value);
        await _unitOfWork.SaveChanges(cancellationToken);

        var accessToken = _tokenProvider.GenerateAccessToken(oldRefreshSessionResult.Value.User);
        
        var refreshToken = await _tokenProvider
            .GenerateRefreshToken(oldRefreshSessionResult.Value.User, accessToken.Jti, cancellationToken);

        return new LoginResponse(accessToken.AccessToken, refreshToken);
    }
}