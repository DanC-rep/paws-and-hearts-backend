using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.VolunteerRequests.Contracts.Requests;

public record CreateVolunteerRequestRequest(
    int Experience,
    IEnumerable<RequisiteDto> Requisites);