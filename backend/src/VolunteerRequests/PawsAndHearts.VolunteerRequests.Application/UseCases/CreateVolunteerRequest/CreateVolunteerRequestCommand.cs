using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.VolunteerRequests.Contracts.Requests;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.CreateVolunteerRequest;

public record CreateVolunteerRequestCommand(
    Guid UserId, 
    int Experience,
    IEnumerable<RequisiteDto> Requisites) : ICommand
{
    public static CreateVolunteerRequestCommand Create(CreateVolunteerRequestRequest request, Guid userId) =>
        new(userId, request.Experience, request.Requisites);
}