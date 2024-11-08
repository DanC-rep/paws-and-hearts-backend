using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;

namespace PawsAndHearts.VolunteerRequests.Domain.Entities;

public class UserRestriction : Entity<UserRestrictionId>
{
    private UserRestriction(UserRestrictionId id) : base(id)
    {
        
    }

    private UserRestriction(UserRestrictionId id, Guid userId, DateTime bannedUntil) : base(id)
    {
        UserId = userId;
        BannedUntil = bannedUntil;
    }
    
    private const int DEFAULT_BAN_DAYS = 1;
    
    public Guid UserId { get; private set; }
    
    public DateTime BannedUntil { get; private set; }

    public static Result<UserRestriction, Error> Create(
        UserRestrictionId id,
        Guid userId,
        int banDays = DEFAULT_BAN_DAYS)
    {
        if (banDays <= 0)
            return Errors.General.ValueIsInvalid("ban days");
        
        var bannedUntil = DateTime.UtcNow.AddMinutes(banDays);
        
        var userRestriction = new UserRestriction(id, userId, bannedUntil);

        return userRestriction;
    }

    public UnitResult<Error> CheckExpirationOfBan()
    {
        if (BannedUntil > DateTime.UtcNow)
            return Error.Failure("account.banned", "Account banned");

        return Result.Success<Error>();
    }
}