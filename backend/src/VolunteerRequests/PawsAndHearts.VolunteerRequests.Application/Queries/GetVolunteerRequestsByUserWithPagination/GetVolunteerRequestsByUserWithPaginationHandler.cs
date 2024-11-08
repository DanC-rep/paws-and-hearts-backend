using System.Linq.Expressions;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Extensions;
using PawsAndHearts.Core.Models;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Contracts.Dtos;

namespace PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteerRequestsByUserWithPagination;

public class GetVolunteerRequestsByUserWithPaginationHandler
    : IQueryHandler<PagedList<VolunteerRequestDto>, GetVolunteerRequestsByUserWithPaginationQuery>
{
    private readonly IVolunteerRequestsReadDbContext _volunteerRequestsReadDbContext;

    public GetVolunteerRequestsByUserWithPaginationHandler(
        IVolunteerRequestsReadDbContext volunteerRequestsReadDbContext)
    {
        _volunteerRequestsReadDbContext = volunteerRequestsReadDbContext;
    }
    
    public async Task<PagedList<VolunteerRequestDto>> Handle(
        GetVolunteerRequestsByUserWithPaginationQuery query, 
        CancellationToken cancellationToken = default)
    {
        var volunteersRequestsQuery = _volunteerRequestsReadDbContext.VolunteerRequests
            .Where(v => v.UserId == query.UserId);
        
        var keySelector = SortByProperty(query.SortBy);
        
        volunteersRequestsQuery = query.SortDirection?.ToLower() == "desc" 
            ? volunteersRequestsQuery.OrderByDescending(keySelector)
            : volunteersRequestsQuery.OrderBy(keySelector);

        volunteersRequestsQuery = volunteersRequestsQuery
            .WhereIf(!string.IsNullOrWhiteSpace(query.RequestStatus),
                request => request.Status == query.RequestStatus);
        
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