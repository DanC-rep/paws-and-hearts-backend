using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PawsAndHearts.Accounts.Application.Queries.GetUserById;
using PawsAndHearts.Accounts.Application.UseCases.Login;
using PawsAndHearts.Accounts.Application.UseCases.RefreshTokens;
using PawsAndHearts.Accounts.Application.UseCases.Register;
using PawsAndHearts.Accounts.Application.UseCases.UpdateUserSocialNetworks;
using PawsAndHearts.Accounts.Application.UseCases.UpdateVolunteerRequisites;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Accounts.Contracts.Requests;
using PawsAndHearts.Accounts.Contracts.Responses;
using PawsAndHearts.Core.Models;
using PawsAndHearts.Framework;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.Framework.Extensions;
using PawsAndHearts.SharedKernel;

namespace PawsAndHearts.Accounts.Presentation;

public class AccountsController : ApplicationController
{
    [HttpPost("registration")]
    public async Task<ActionResult> Register(
        [FromBody] RegisterUserRequest request,
        [FromServices] RegisterUserHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = RegisterUserCommand.Create(request);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginUserRequest request,
        [FromServices] LoginUserHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = LoginUserCommand.Create(request);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> RefreshTokens(
        [FromBody] RefreshTokensRequest request,
        [FromServices] RefreshTokensHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = RefreshTokensCommand.Create(request);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("user.update")]
    [HttpPut("user/social-networks")]
    public async Task<ActionResult> UpdateUserSocialNetworks(
        [FromBody] UpdateUserSocialNetworksRequest request,
        [FromServices] UpdateUserSocialNetworksHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();
        
        var command = UpdateUserSocialNetworksCommand.Create(request, userIdResult.Value);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("volunteer.account.update")]
    [HttpPut("volunteer-account/requisites")]
    public async Task<ActionResult> UpdateVolunteerRequisites(
        [FromBody] UpdateVolunteerRequisitesRequest request,
        [FromServices] UpdateVolunteerRequisitesHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = UpdateVolunteerRequisitesCommand.Create(request, userIdResult.Value);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetUserById(
        [FromRoute] Guid userId,
        [FromServices] GetUserByIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(userId);
        
        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }
}