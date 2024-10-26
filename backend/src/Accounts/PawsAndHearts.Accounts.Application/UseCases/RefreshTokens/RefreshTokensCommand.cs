using PawsAndHearts.Accounts.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.Application.UseCases.RefreshTokens;

public record RefreshTokensCommand(string AccessToken, Guid RefreshToken) : ICommand
{
    public static RefreshTokensCommand Create(RefreshTokensRequest request) =>
        new(request.AccessToken, request.RefreshToken);
}