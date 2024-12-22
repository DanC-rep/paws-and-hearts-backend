using PawsAndHearts.Accounts.Contracts.Dtos;
using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.Accounts.Contracts.Responses;

public class GetUserByIdResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public string Surname { get; init; } = default!;

    public string? Patronymic { get; init; } = default!;

    public string UserName { get; init; } = default!;
    
    public Guid? PhotoId { get; init; }

    public string? DownloadPhotoPath { get; init; } = default!;

    public List<RoleDto> Roles { get; init; } = [];

    public IEnumerable<SocialNetworkDto>? SocialNetworks { get; init; } = [];
    
    public AdminAccountDto? AdminAccount { get; set; }
    
    public ParticipantAccountDto? ParticipantAccount { get; set; }
    
    public VolunteerAccountDto? VolunteerAccount { get; set; }
    
    public static GetUserByIdResponse Create(UserDto user, string? photoUrl = null) =>
        new GetUserByIdResponse()
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Patronymic = user.Patronymic,
            UserName = user.UserName,
            PhotoId = user.PhotoId,
            DownloadPhotoPath = photoUrl,
            Roles = user.Roles,
            SocialNetworks = user.SocialNetworks,
            AdminAccount = user.AdminAccount,
            ParticipantAccount = user.ParticipantAccount,
            VolunteerAccount = user.VolunteerAccount
        };
}