using MediatR;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.Events;

namespace PawsAndHearts.VolunteerRequests.Application.EventHandlers.UserRestrictionOnVolunteerRequestChecked;

public class UserRestrictionOnVolunteerRequestChecked : 
    INotificationHandler<UserRestrictionOnVolunteerRequestCheckedEvent>
{
    private readonly IUserRestrictionRepository _userRestrictionRepository;

    public UserRestrictionOnVolunteerRequestChecked(IUserRestrictionRepository userRestrictionRepository)
    {
        _userRestrictionRepository = userRestrictionRepository;
    }
    
    public async Task Handle(
        UserRestrictionOnVolunteerRequestCheckedEvent notification, 
        CancellationToken cancellationToken = default)
    {
        var userRestrictionResult = await _userRestrictionRepository
            .GetByUserId(notification.UserId, cancellationToken);

        if (userRestrictionResult.IsSuccess)
        {
            var checkBanResult = userRestrictionResult.Value.CheckExpirationOfBan();

            if (checkBanResult.IsFailure)
                throw new AccountBannedException(checkBanResult.Error);

            var deleteResult = _userRestrictionRepository.Delete(userRestrictionResult.Value, cancellationToken);

            if (deleteResult.IsFailure)
                throw new DeleteRecordException(deleteResult.Error);
        }
    }
}