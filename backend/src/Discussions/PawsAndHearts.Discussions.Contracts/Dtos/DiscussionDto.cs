namespace PawsAndHearts.Discussions.Contracts.Dtos;

public class DiscussionDto
{
    public Guid Id { get; init; }
    
    public Guid RelationId { get; init; }
    
    public Guid FirstMember { get; init; }
    
    public Guid SecondMember { get; init; }

    public string Status { get; init; } = default!;

    public List<MessageDto> Messages { get; init; } = default!;
}