using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PawsAndHearts.Core.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Accounts.Application.Interfaces;
using PawsAndHearts.Accounts.Contracts.Responses;
using PawsAndHearts.Accounts.Domain;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Application.UseCases.Login;

public class LoginUserHandler : ICommandHandler<LoginResponse, LoginUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<LoginUserHandler> _logger;

    public LoginUserHandler(
        UserManager<User> userManager,
        ITokenProvider tokenProvider,
        ILogger<LoginUserHandler> logger)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public async Task<Result<LoginResponse, ErrorList>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
            return Errors.General.NotFound(null, "user").ToErrorList();
        
        var passwordConfirmed = await _userManager.CheckPasswordAsync(user, command.Password);

        if (!passwordConfirmed)
            return Errors.Accounts.InvalidCredentials().ToErrorList();

        var accessToken = _tokenProvider.GenerateAccessToken(user);
        var refreshToken = await _tokenProvider.GenerateRefreshToken(user, accessToken.Jti, cancellationToken);
        
        _logger.LogInformation("Successfully logged in");

        var userResponse = new UserResponse(
            user.Id, 
            user.Email!, 
            user.UserName!, 
            user.Roles.Select(r => r.Name!.ToLower()));

        return new LoginResponse(accessToken.AccessToken, refreshToken, userResponse);
    }
}