using CSharpFunctionalExtensions;
using FileService.Contracts.Requests;
using FileService.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using PawsAndHearts.Accounts.Application.Queries.GetUserById;
using PawsAndHearts.Accounts.Application.UseCases.CompleteUploadPhoto;
using PawsAndHearts.Accounts.Application.UseCases.Login;
using PawsAndHearts.Accounts.Application.UseCases.RefreshTokens;
using PawsAndHearts.Accounts.Application.UseCases.Register;
using PawsAndHearts.Accounts.Application.UseCases.StartUploadPhoto;
using PawsAndHearts.Accounts.Application.UseCases.UpdateUserSocialNetworks;
using PawsAndHearts.Accounts.Application.UseCases.UpdateVolunteerRequisites;
using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Accounts.Contracts.Requests;
using PawsAndHearts.Accounts.Contracts.Responses;
using PawsAndHearts.Framework;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.Framework.Extensions;

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
        
        if (result.IsSuccess)
            CookieManager.AddRefreshTokenToCookies(HttpContext, result.Value.RefreshToken);

        return result.ToResponse();
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> RefreshTokens(
        [FromServices] RefreshTokensHandler handler,
        CancellationToken cancellationToken = default)
    {
        var refreshToken = CookieManager.GetRefreshToken(HttpContext);

        if (refreshToken.IsFailure)
            return UnitResult.Failure(refreshToken.Error).ToResponse();

        var command = new RefreshTokensCommand(refreshToken.Value);
        
        var result = await handler.Handle(command, cancellationToken);
        
        if (result.IsSuccess)
            CookieManager.AddRefreshTokenToCookies(HttpContext, result.Value.RefreshToken);

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
    public async Task<ActionResult<GetUserByIdResponse>> GetUserById(
        [FromRoute] Guid userId,
        [FromServices] GetUserByIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(userId);
        
        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }

    [Permission("user.update")]
    [HttpPost("start-upload-photo")]
    public async Task<ActionResult<StartMultipartUploadResponse>> StartUploadPhoto(
        [FromBody] StartMultipartUploadRequest request,
        [FromServices] StartUploadPhotoHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = StartUploadPhotoCommand.Create(userIdResult.Value, request);

        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("user.update")]
    [HttpPost("{key}/complete-multipart-upload")]
    public async Task<ActionResult<CompleteMultipartUploadResponse>> CompleteUploadPhoto(
        [FromRoute] string key,
        [FromBody] CompleteMultipartRequest request,
        [FromServices] CompleteUploadPhotoHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = CompleteUploadPhotoCommand.Create(userIdResult.Value, key, request);

        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }
    
}