using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.VolunteerRequests.Contracts.Messaging;

public record ApproveVolunteerRequestEvent(Guid UserId, int Experience, IEnumerable<RequisiteDto> Requisites);