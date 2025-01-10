using PawsAndHearts.SharedKernel.Interfaces;

namespace PawsAndHearts.VolunteerRequests.Domain.Events;

public record VolunteerRequestRejectedEvent(Guid UserId) : IDomainEvent;