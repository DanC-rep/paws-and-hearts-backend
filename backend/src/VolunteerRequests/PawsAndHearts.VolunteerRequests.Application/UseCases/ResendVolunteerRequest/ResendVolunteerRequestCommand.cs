using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.ResendVolunteerRequest;

public record ResendVolunteerRequestCommand(Guid UserId, Guid VolunteerRequestId) : ICommand;