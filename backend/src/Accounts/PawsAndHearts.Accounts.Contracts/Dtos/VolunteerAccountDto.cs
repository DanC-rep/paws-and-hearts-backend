using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.Accounts.Contracts.Dtos;

public class VolunteerAccountDto
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public int Experience { get; init; }

    public IEnumerable<RequisiteDto> Requisites { get; init; } = [];
}