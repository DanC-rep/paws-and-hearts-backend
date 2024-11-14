namespace PawsAndHearts.PetManagement.Contracts.Dtos;

public class VolunteerDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public string Surname { get; init; } = default!;

    public string? Patronymic { get; init; }
    
    public int Experience { get; init; }

    public string PhoneNumber { get; init; } = default!;
    
    public bool IsDeleted { get; init; }
}