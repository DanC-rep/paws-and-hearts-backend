using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Repositories;

public class VolunteerRequestsRepository : IVolunteerRequestsRepository
{
    private readonly WriteDbContext _writeDbContext;

    public VolunteerRequestsRepository(WriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }
    
    public async Task<Guid> Add(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.AddAsync(volunteerRequest, cancellationToken);

        return volunteerRequest.Id;
    }

    public async Task<Result<VolunteerRequest, Error>> GetById(
        VolunteerRequestId volunteerRequestId, 
        CancellationToken cancellationToken = default)
    {
        var volunteerRequest = await _writeDbContext.VolunteerRequests
            .FirstOrDefaultAsync(d => d.Id == volunteerRequestId, cancellationToken);

        if (volunteerRequest is null)
            return Errors.General.NotFound(volunteerRequestId);

        return volunteerRequest;
    }

    public Result<Guid, Error> Delete(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default)
    {
        _writeDbContext.VolunteerRequests.Remove(volunteerRequest);

        return (Guid)volunteerRequest.Id;
    }
}