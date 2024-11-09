using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Repositories;

public class UserRestrictionRepository : IUserRestrictionRepository
{
    private readonly WriteDbContext _writeDbContext;

    public UserRestrictionRepository(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }
    
    public async Task<Guid> Add(UserRestriction userRestriction, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.AddAsync(userRestriction, cancellationToken);

        return userRestriction.Id;
    }

    public async Task<Result<UserRestriction, Error>> GetById(
        UserRestrictionId userRestrictionId, 
        CancellationToken cancellationToken = default)
    {
        var userRestriction = await _writeDbContext.UserRestrictions
            .FirstOrDefaultAsync(u => u.Id == userRestrictionId, cancellationToken);

        if (userRestriction is null)
            return Errors.General.NotFound(userRestrictionId);

        return userRestriction;
    }
    
    public async Task<Result<UserRestriction, Error>> GetByUserId(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var userRestriction = await _writeDbContext.UserRestrictions
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (userRestriction is null)
            return Errors.General.NotFound(null, "user restriction");

        return userRestriction;
    }

    public Result<Guid, Error> Delete(UserRestriction userRestriction, CancellationToken cancellationToken = default)
    {
        _writeDbContext.UserRestrictions.Remove(userRestriction);

        return (Guid)userRestriction.Id;
    }
}