using PawsAndHearts.Accounts.Contracts.Requests;
using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.Application.UseCases.RefreshTokens;

public record RefreshTokensCommand(Guid RefreshToken) : ICommand;
