using CSharpFunctionalExtensions;
using PawsAndHearts.Discussions.Domain.Enums;
using PawsAndHearts.Discussions.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Domain.Entities;

public class Discussion : Entity<DiscussionId>
{
    public Guid RelationId { get; private set; }
    
    public Users Users { get; private set; }
    
    public DiscussionStatus Status { get; private set; }
    
    private readonly List<Message> _messages = [];

    public IReadOnlyList<Message> Messages => _messages;

    private Discussion(DiscussionId id) : base(id)
    {
    }

    public Discussion(DiscussionId id, Users users, Guid relationId) : base(id)
    {
        Users = users;
        RelationId = relationId;
        Status = DiscussionStatus.Open;
    }

    public UnitResult<Error> SendComment(Message message)
    {
        if (message.UserId != Users.FirstMember && message.UserId != Users.SecondMemeber)
            return Error.Failure("access.denied", "Send comment can user that take part in discussion");
        
        _messages.Add(message);

        return Result.Success<Error>();
    }

    public UnitResult<Error> DeleteComment(Guid userId, MessageId messageId)
    {
        var messageResult = GetMessageById(messageId);

        if (messageResult.IsFailure)
            return messageResult.Error;

        if (messageResult.Value.UserId != userId)
            return Error.Failure("access.denied", "Delete comment can user that send this message");

        _messages.Remove(messageResult.Value);

        return Result.Success<Error>();
    }

    public UnitResult<Error> EditComment(Guid userId, MessageId messageId, string text)
    {
        var messageResult = GetMessageById(messageId);

        if (messageResult.Value is null)
            return Errors.General.NotFound(messageId);
        
        if (messageResult.Value.UserId != userId)
            return Error.Failure("access.denied", "Edit comment can user that send this message");
        
        messageResult.Value.Edit(text);
        
        return Result.Success<Error>();
    }

    public UnitResult<Error> CloseDiscussion()
    {
        if (Status != DiscussionStatus.Open)
            return Errors.General.ValueIsInvalid("discussion status");
        
        Status = DiscussionStatus.Closed;

        return Result.Success<Error>();
    }

    private Result<Message, Error> GetMessageById(MessageId messageId)
    {
        var message = _messages.FirstOrDefault(m => m.Id == messageId);

        if (message is null)
            return Errors.General.NotFound(messageId);

        return message;
    }
}