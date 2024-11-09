using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.VolunteerRequests.Contracts.Requests;

public record UpdateVolunteerRequestRequest(
    int Experience,
    IEnumerable<RequisiteDto> Requisites);