using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.RejectVolunteerRequest;

public record RejectVolunteerRequestCommand(
    Guid VolunteerRequestId, Guid  UserId, string RejectionComment) : ICommand;