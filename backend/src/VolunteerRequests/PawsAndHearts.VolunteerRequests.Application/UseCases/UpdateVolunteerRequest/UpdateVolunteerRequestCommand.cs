using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.UpdateVolunteerRequest;

public record UpdateVolunteerRequestCommand(
    Guid UserId, 
    Guid VolunteerRequestId, 
    int Experience,
    IEnumerable<RequisiteDto> Requisites) : ICommand;