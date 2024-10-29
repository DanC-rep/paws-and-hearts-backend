namespace PawsAndHearts.Accounts.Contracts.Dtos;

public class UserRolesDto
{
    public RoleDto Role { get; init; }
    public Guid RoleId { get; init; }
    
    public UserDto User { get; init; }
    public Guid UserId { get; init; }
}