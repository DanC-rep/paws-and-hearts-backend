using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.Accounts.Contracts.Dtos;

public class UserDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public string Surname { get; init; } = default!;

    public string? Patronymic { get; init; } = default!;

    public string UserName { get; init; } = default!;
    
    public Guid? PhotoId { get; init; }

    public List<RoleDto> Roles { get; init; } = [];

    public IEnumerable<SocialNetworkDto>? SocialNetworks { get; init; } = [];
    
    public AdminAccountDto? AdminAccount { get; set; }
    
    public ParticipantAccountDto? ParticipantAccount { get; set; }
    
    public VolunteerAccountDto? VolunteerAccount { get; set; }
}