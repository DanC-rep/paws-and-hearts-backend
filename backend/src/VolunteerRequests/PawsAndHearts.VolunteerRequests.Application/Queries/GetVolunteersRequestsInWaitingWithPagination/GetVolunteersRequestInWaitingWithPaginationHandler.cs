using System.Linq.Expressions;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Contracts.Dtos;
using PawsAndHearts.VolunteerRequests.Domain.Enums;

namespace PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteersRequestsInWaitingWithPagination;

public class GetVolunteersRequestInWaitingWithPaginationHandler : 
    IQueryHandler<PagedList<VolunteerRequestDto>, GetVolunteersRequestsInWaitingWithPaginationQuery>
{
    private readonly IVolunteerRequestsReadDbContext _volunteerRequestsReadDbContext;

    public GetVolunteersRequestInWaitingWithPaginationHandler(
        IVolunteerRequestsReadDbContext volunteerRequestsReadDbContext)
    {
        _volunteerRequestsReadDbContext = volunteerRequestsReadDbContext;
    }
    
    public async Task<PagedList<VolunteerRequestDto>> Handle(
        GetVolunteersRequestsInWaitingWithPaginationQuery query, 
        CancellationToken cancellationToken = default)
    {
        var volunteersRequestsQuery = _volunteerRequestsReadDbContext.VolunteerRequests;

        var keySelector = SortByProperty(query.SortBy);
        
        volunteersRequestsQuery = query.SortDirection?.ToLower() == "desc" 
            ? volunteersRequestsQuery.OrderByDescending(keySelector)
            : volunteersRequestsQuery.OrderBy(keySelector);

        var statusValue = VolunteerRequestStatus.Waiting.ToString();

        volunteersRequestsQuery = volunteersRequestsQuery.Where(v => v.Status == statusValue);
        
        var result = await volunteersRequestsQuery.ToPagedList(query.Page, query.PageSize, cancellationToken);

        return result;

    }
    
    private static Expression<Func<VolunteerRequestDto, object>> SortByProperty(string? sortBy)
    {
        if (string.IsNullOrEmpty(sortBy))
            return request => request.Id;

        Expression<Func<VolunteerRequestDto, object>> keySelector = sortBy?.ToLower() switch
        {
            "status" => request => request.Status,
            "createdAt" => request => request.CreatedAt,
            "experience" => request => request.Experience,
            _ => request => request.Id
        };
        
        return keySelector;
    }
}