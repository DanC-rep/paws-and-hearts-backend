using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Domain.Entities;

namespace PawsAndHearts.VolunteerRequests.Application.Interfaces;

public interface IUserRestrictionRepository
{
    Task<Guid> Add(UserRestriction userRestriction, CancellationToken cancellationToken = default);
    
    Task<Result<UserRestriction, Error>> GetById(
        UserRestrictionId userRestrictionId, 
        CancellationToken cancellationToken = default);
    
    Task<Result<UserRestriction, Error>> GetByUserId(
        Guid userId, 
        CancellationToken cancellationToken = default);
    
    Result<Guid, Error> Delete(UserRestriction userRestriction, CancellationToken cancellationToken = default);
}