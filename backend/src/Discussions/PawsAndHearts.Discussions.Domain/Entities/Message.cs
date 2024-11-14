using CSharpFunctionalExtensions;
using PawsAndHearts.Discussions.Domain.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.Discussions.Domain.Entities;

public class Message : Entity<MessageId>
{
    public Guid UserId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    public MessageText Text { get; private set; } = default!;
    
    public bool IsEdited { get; private set; }

    public DiscussionId DiscussionId { get; private set; }

    private Message(MessageId id) : base(id)
    {
    }

    public Message(MessageId id, Guid userId, MessageText text, DateTime createdAt) : base(id)
    {
        UserId = userId;
        Text = text;
        IsEdited = false;
        CreatedAt = createdAt;
    }

    internal void Edit(MessageText text)
    {
        Text = text;
        IsEdited = true;
    }
}