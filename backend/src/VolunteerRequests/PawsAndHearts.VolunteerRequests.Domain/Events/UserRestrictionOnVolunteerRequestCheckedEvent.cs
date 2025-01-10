using PawsAndHearts.SharedKernel.Interfaces;

namespace PawsAndHearts.VolunteerRequests.Domain.Events;

public record UserRestrictionOnVolunteerRequestCheckedEvent(Guid UserId) : IDomainEvent;