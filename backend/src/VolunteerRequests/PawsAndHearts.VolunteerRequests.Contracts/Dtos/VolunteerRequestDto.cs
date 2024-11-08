using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.VolunteerRequests.Contracts.Dtos;

public class VolunteerRequestDto
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public Guid AdminId { get; init; }
    
    public Guid DiscussionId { get; init; }

    public string Status { get; init; } = default!;
    
    public DateTime CreatedAt { get; init; }

    public string RejectionComment { get; init; } = default!;
    
    public int Experience { get; init; }

    public IEnumerable<RequisiteDto> Requisites { get; init; } = [];
}