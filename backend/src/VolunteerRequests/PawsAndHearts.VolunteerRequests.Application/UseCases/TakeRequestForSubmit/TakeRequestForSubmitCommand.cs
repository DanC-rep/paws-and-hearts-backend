using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.VolunteerRequests.Application.UseCases.TakeRequestForSubmit;

public record TakeRequestForSubmitCommand(Guid VolunteerRequestId, Guid AdminId) : ICommand;