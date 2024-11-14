namespace PawsAndHearts.Discussions.Contracts.Dtos;

public class MessageDto
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public DateTime CreatedAt { get; init; }

    public string Text { get; init; } = default!;
    
    public bool IsEdited { get; init; }
    
    public Guid DiscussionId { get; init; }
}