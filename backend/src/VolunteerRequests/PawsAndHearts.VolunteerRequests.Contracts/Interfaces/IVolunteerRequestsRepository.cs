using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Domain.Entities;

namespace PawsAndHearts.VolunteerRequests.Contracts.Interfaces;

public interface IVolunteerRequestsRepository
{
    Task<Guid> Add(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default);
    
    Task<Result<VolunteerRequest, Error>> GetById(
        VolunteerRequestId volunteerRequestId, 
        CancellationToken cancellationToken = default);
    
    Result<Guid, Error> Delete(VolunteerRequest volunteerRequest, CancellationToken cancellationToken = default);
}