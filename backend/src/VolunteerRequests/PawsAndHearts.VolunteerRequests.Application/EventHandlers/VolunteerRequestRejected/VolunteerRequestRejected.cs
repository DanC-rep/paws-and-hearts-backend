using MediatR;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Domain.Events;

namespace PawsAndHearts.VolunteerRequests.Application.EventHandlers.VolunteerRequestRejected;

public class VolunteerRequestRejected : INotificationHandler<VolunteerRequestRejectedEvent>
{
    private readonly IUserRestrictionRepository _userRestrictionRepository;

    public VolunteerRequestRejected(IUserRestrictionRepository userRestrictionRepository)
    {
        _userRestrictionRepository = userRestrictionRepository;
    }
    
    public async Task Handle(
        VolunteerRequestRejectedEvent notification, 
        CancellationToken cancellationToken = default)
    {
        var userRestrictionId = UserRestrictionId.NewId();

        var userRestrictionResult = UserRestriction
            .Create(userRestrictionId, notification.UserId);

        if (userRestrictionResult.IsFailure)
            throw new Exception(userRestrictionResult.Error.Message);

        await _userRestrictionRepository.Add(userRestrictionResult.Value, cancellationToken);
    }
}