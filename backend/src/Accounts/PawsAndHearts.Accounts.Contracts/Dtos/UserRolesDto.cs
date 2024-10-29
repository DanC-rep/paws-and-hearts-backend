namespace PawsAndHearts.Accounts.Contracts.Dtos;

public class UserRolesDto
{
    public RoleDto Role { get; init; } = default!;
    public Guid RoleId { get; init; }

    public UserDto User { get; init; } = default!;
    public Guid UserId { get; init; }
}