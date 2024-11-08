using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.ApproveVolunteerRequest;

public record ApproveVolunteerRequestCommand(Guid VolunteerRequestId, Guid AdminId) : ICommand;