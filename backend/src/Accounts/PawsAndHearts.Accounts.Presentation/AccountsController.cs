using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PawsAndHearts.Accounts.Application.UseCases.Login;
using PawsAndHearts.Accounts.Application.UseCases.RefreshTokens;
using PawsAndHearts.Accounts.Application.UseCases.Register;
using PawsAndHearts.Accounts.Application.UseCases.UpdateUserSocialNetworks;
using PawsAndHearts.Accounts.Application.UseCases.UpdateVolunteerRequisites;
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
        CancellationToken cancellationToken = default)
    {
        var userIdString = HttpContext.User.Claims.FirstOrDefault(u => u.Type == CustomClaims.Id)?.Value;

        if (userIdString is null)
        {
            var error = Errors.General.NotFound(null, "user id").ToErrorList();
            return UnitResult.Failure(error).ToResponse();
        }

        if (!Guid.TryParse(userIdString, out var userId))
        {
            var error = Error.Failure("parse.error", "can not convert user id to guid").ToErrorList();
            return UnitResult.Failure(error).ToResponse();
        }
        
        var command = UpdateUserSocialNetworksCommand.Create(request, userId);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("volunteer.account.update")]
    [HttpPut("volunteer-account/requisites")]
    public async Task<ActionResult> UpdateVolunteerRequisites(
        [FromBody] UpdateVolunteerRequisitesRequest request,
        [FromServices] UpdateVolunteerRequisitesHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userIdString = HttpContext.User.Claims.FirstOrDefault(u => u.Type == CustomClaims.Id)?.Value;

        if (userIdString is null)
        {
            var error = Errors.General.NotFound(null, "user id").ToErrorList();
            return UnitResult.Failure(error).ToResponse();
        }

        if (!Guid.TryParse(userIdString, out var userId))
        {
            var error = Error.Failure("parse.error", "can not convert user id to guid").ToErrorList();
            return UnitResult.Failure(error).ToResponse();
        }

        var command = UpdateVolunteerRequisitesCommand.Create(request, userId);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }
}