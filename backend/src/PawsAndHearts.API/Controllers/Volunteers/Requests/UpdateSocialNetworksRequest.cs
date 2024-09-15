using PawsAndHearts.Application.Dto;
using PawsAndHearts.Application.Services.Volunteers.UpdateSocialNetworks;

namespace PawsAndHearts.API.Controllers.Volunteers.Requests;

public record UpdateSocialNetworksRequest(IEnumerable<SocialNetworkDto> SocialNetworks)
{
    public UpdateSocialNetworksCommand ToCommand(Guid volunteerId) =>
        new(volunteerId, SocialNetworks);
}