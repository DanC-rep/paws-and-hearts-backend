using PawsAndHearts.VolunteerRequests.Contracts.Dtos;

namespace PawsAndHearts.VolunteerRequests.Application.Interfaces;

public interface IVolunteerRequestsReadDbContext
{
    public IQueryable<VolunteerRequestDto> VolunteerRequests { get; }
}