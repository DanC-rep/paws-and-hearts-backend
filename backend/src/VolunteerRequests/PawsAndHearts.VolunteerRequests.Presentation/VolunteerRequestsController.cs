using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PawsAndHearts.Core.Models;
using PawsAndHearts.Framework;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.Framework.Extensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteerRequestsByUserWithPagination;
using PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteersRequestsByAdminWithPagination;
using PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteersRequestsInWaitingWithPagination;
using PawsAndHearts.VolunteerRequests.Application.UseCases.ApproveVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Application.UseCases.CreateVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Application.UseCases.RejectVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Application.UseCases.ResendVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Application.UseCases.SendVolunteerRequestForRevision;
using PawsAndHearts.VolunteerRequests.Application.UseCases.TakeRequestForSubmit;
using PawsAndHearts.VolunteerRequests.Application.UseCases.UpdateVolunteerRequest;
using PawsAndHearts.VolunteerRequests.Contracts.Requests;

namespace PawsAndHearts.VolunteerRequests.Presentation;

public class VolunteerRequestsController : ApplicationController
{
    [Permission("volunteer.request.create")]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateVolunteerRequest(
        [FromBody] CreateVolunteerRequestRequest request,
        [FromServices] CreateVolunteerRequestHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();
        
        var command = CreateVolunteerRequestCommand.Create(request, userIdResult.Value);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("volunteer.request.review")]
    [HttpPost("{volunteerRequestId:guid}/taking-for-submitting")]
    public async Task<ActionResult> TakeVolunteerRequestForSubmit(
        [FromRoute] Guid volunteerRequestId,
        [FromServices] TakeRequestForSubmitHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = new TakeRequestForSubmitCommand(volunteerRequestId, userIdResult.Value);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("volunteer.request.review")]
    [HttpPost("{volunteerRequestId:guid}/sending-for-revision")]
    public async Task<ActionResult> SendVolunteerRequestForRevision(
        [FromRoute] Guid volunteerRequestId,
        [FromBody] SendVolunteerRequestForRevisionRequest request,
        [FromServices] SendVolunteerRequestForRevisionHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = new SendVolunteerRequestForRevisionCommand(
            userIdResult.Value, volunteerRequestId, request.RejectionComment);

        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }


    [Permission("volunteer.request.review")]
    [HttpPost("{volunteerRequestId:guid}/approving")]
    public async Task<ActionResult> ApproveVolunteerRequest(
        [FromRoute] Guid volunteerRequestId,
        [FromServices] ApproveVolunteerRequestHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = new ApproveVolunteerRequestCommand(volunteerRequestId, userIdResult.Value);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }
    
    [Permission("volunteer.request.review")]
    [HttpPost("{volunteerRequestId:guid}/rejecting")]
    public async Task<ActionResult> RejectVolunteerRequest(
        [FromRoute] Guid volunteerRequestId,
        [FromBody] RejectVolunteerRequestRequest request,
        [FromServices] RejectVolunteerRequestHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = new RejectVolunteerRequestCommand(
            volunteerRequestId, userIdResult.Value, request.RejectionComment);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("volunteer.request.update")]
    [HttpPut("{volunteerRequestId:guid}")]
    public async Task<ActionResult> UpdateVolunteerRequest(
        [FromRoute] Guid volunteerRequestId,
        [FromBody] UpdateVolunteerRequestRequest request,
        [FromServices] UpdateVolunteerRequestHandler handler,
        ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();
        
        var command = new UpdateVolunteerRequestCommand(
            userIdResult.Value, volunteerRequestId, request.Experience, request.Requisites);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("volunteer.request.update")]
    [HttpPost("{volunteerRequestId:guid}/resending")]
    public async Task<ActionResult> ResendVolunteerRequest(
        Guid volunteerRequestId,
        [FromServices] ResendVolunteerRequestHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();

        var command = new ResendVolunteerRequestCommand(userIdResult.Value, volunteerRequestId);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("volunteer.request.read")]
    [HttpGet("in-waiting")]
    public async Task<ActionResult> GetVolunteerRequestsInWaiting(
        [FromQuery] GetVolunteersRequestsInWaitingWithPaginationRequest request,
        [FromServices] GetVolunteersRequestInWaitingWithPaginationHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = GetVolunteersRequestsInWaitingWithPaginationQuery.Create(request);

        var response = await handler.Handle(query, cancellationToken);

        return Ok(response);
    }

    [Permission("volunteer.request.read")]
    [HttpGet("by-admin")]
    public async Task<ActionResult> GetVolunteersRequestsByAdmin(
        [FromQuery] GetVolunteersRequestsByAdminWithPaginationRequest request,
        [FromServices] GetVolunteersRequestsByAdminWithPaginationHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();
        
        var query = GetVolunteersRequestsByAdminWithPaginationQuery.Create(request, userIdResult.Value);
        
        var response = await handler.Handle(query, cancellationToken);
        
        return Ok(response);
    }

    [Permission("volunteer.request.read.own")]
    [HttpGet("by-user")]
    public async Task<ActionResult> GetVolunteerRequestsByUser(
        [FromQuery] GetVolunteerRequestsByUserWithPaginationRequest request,
        [FromServices] GetVolunteerRequestsByUserWithPaginationHandler handler,
        [FromServices] ClaimsManager claimsManager,
        CancellationToken cancellationToken = default)
    {
        var userIdResult = claimsManager.GetCurrentUserId(HttpContext);

        if (userIdResult.IsFailure)
            return UnitResult.Failure(userIdResult.Error).ToResponse();
        
        var query = GetVolunteerRequestsByUserWithPaginationQuery.Create(request, userIdResult.Value);
        
        var response = await handler.Handle(query, cancellationToken);
        
        return Ok(response);
    }
}