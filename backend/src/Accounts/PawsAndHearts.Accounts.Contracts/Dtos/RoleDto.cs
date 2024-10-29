namespace PawsAndHearts.Accounts.Contracts.Dtos;

public class RoleDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;
    
    public List<UserDto> Users { get; init; } = [];

    public List<UserRolesDto> UserRoles { get; init; } = [];
}