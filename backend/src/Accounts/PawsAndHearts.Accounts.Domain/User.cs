using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Domain;

public class User : IdentityUser<Guid>
{
    public Photo? Photo { get; set; }
    
    public FullName FullName { get; set; }
    
    public IReadOnlyList<SocialNetwork>? SocialNetworks { get; set; }

    public List<Role> Roles { get; set; } = [];

    public static Result<User, Error> CreateParticipant(
        string userName,
        string email,
        FullName fullName,
        List<SocialNetwork> socialNetworks,
        Role role)
    {
        if (role.Name != Constants.PARTICIPANT)
            return Errors.Accounts.InvalidRole();

        return new User
        {
            UserName = userName,
            Email = email,
            FullName = fullName,
            SocialNetworks = socialNetworks,
            Roles = [role]
        };
    }

    public static Result<User, Error> CreateAdminAccount(
        string userName,
        string email,
        FullName fullName,
        Role role)
    {
        if (role.Name != Constants.ADMIN)
            return Errors.Accounts.InvalidRole();

        return new User
        {
            UserName = userName,
            Email = email,
            FullName = fullName,
            Roles = [role]
        };
    }

    public void UpdateSocialNetworks(List<SocialNetwork> socialNetworks)
    {
        SocialNetworks = socialNetworks;
    }
    
}
