using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.SendVolunteerRequestForRevision;

public record SendVolunteerRequestForRevisionCommand(
    Guid AdminId, Guid VolunteerRequestId, string RejectionComment) : ICommand;