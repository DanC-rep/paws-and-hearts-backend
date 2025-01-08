using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PawsAndHearts.Discussions.Application.Queries.GetDiscussionByRelationId;
using PawsAndHearts.Discussions.Application.UseCases.CloseDiscussion;
using PawsAndHearts.Discussions.Application.UseCases.DeleteMessage;
using PawsAndHearts.Discussions.Application.UseCases.SendMessage;
using PawsAndHearts.Discussions.Application.UseCases.UpdateMessage;
using PawsAndHearts.Discussions.Contracts.Dtos;
using PawsAndHearts.Discussions.Contracts.Requests;
using PawsAndHearts.Framework;
using PawsAndHearts.Framework.Authorization;
using PawsAndHearts.Framework.Extensions;

namespace PawsAndHearts.Discussions.Presentation;

public class DiscussionsController : ApplicationController
{
    [Permission("discussion.send.message")]
    [HttpPost("{discussionId:guid}/sending-message")]
    public async Task<ActionResult<Guid>> SendMessage(
        [FromRoute] Guid discussionId,
        [FromBody] SendMessageRequest request,
        [FromServices] SendMessageHandler handler,
        [FromServices] UserScopedData userScopedData,
        CancellationToken cancellationToken = default)
    {
        var command = new SendMessageCommand(discussionId, userScopedData.UserId, request.Message);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("discussion.update.message")]
    [HttpPut("{discussionId:guid}/{messageId:guid}")]
    public async Task<ActionResult> UpdateMessage(
        [FromRoute] Guid discussionId,
        [FromRoute] Guid messageId,
        [FromBody] UpdateMessageRequest request,
        [FromServices] UpdateMessageHandler handler,
        [FromServices] UserScopedData userScopedData,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMessageCommand(discussionId, messageId, userScopedData.UserId, request.Message);

        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("discussion.delete.message")]
    [HttpDelete("{discussionId:guid}/{messageId:guid}")]
    public async Task<ActionResult> DeleteMessage(
        [FromRoute] Guid discussionId,
        [FromRoute] Guid messageId,
        [FromServices] DeleteMessageHandler handler,
        [FromServices] UserScopedData userScopedData,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteMessageCommand(discussionId, messageId, userScopedData.UserId);

        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("discussion.close")]
    [HttpPost("{discussionId:guid}/closing")]
    public async Task<ActionResult> CloseDiscussion(
        [FromRoute] Guid discussionId,
        [FromServices] CloseDiscussionHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new CloseDiscussionCommand(discussionId);
        
        var result = await handler.Handle(command, cancellationToken);

        return result.ToResponse();
    }

    [Permission("discussion.read")]
    [HttpGet("{relationId:guid}")]
    public async Task<ActionResult<DiscussionDto>> GetByRelationId(
        [FromRoute] Guid relationId,
        [FromServices] GetDiscussionByRelationIdHandler handler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetDiscussionByRelationIdQuery(relationId);

        var result = await handler.Handle(query, cancellationToken);

        return result.ToResponse();
    }
}