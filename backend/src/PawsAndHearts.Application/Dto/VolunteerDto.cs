namespace PawsAndHearts.Application.Dto;

public class VolunteerDto
{
    public Guid Id { get; set; }

    public string Name { get; init; } = default!;

    public string Surname { get; init; } = default!;

    public string? Patronymic { get; init; }
    
    public int Experience { get; init; }

    public string PhoneNumber { get; init; } = default!;

    public IEnumerable<SocialNetworkDto> SocialNetworks { get; init; } = [];
    
    public IEnumerable<RequisiteDto> Requisites { get; init; } = [];
}