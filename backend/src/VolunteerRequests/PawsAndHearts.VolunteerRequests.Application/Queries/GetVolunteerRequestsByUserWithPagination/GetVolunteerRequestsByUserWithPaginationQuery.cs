using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Contracts.Requests;

namespace PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteerRequestsByUserWithPagination;

public record GetVolunteerRequestsByUserWithPaginationQuery(
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize,
    string? RequestStatus,
    Guid UserId) : IQuery
{
    public static GetVolunteerRequestsByUserWithPaginationQuery Create(
        GetVolunteerRequestsByUserWithPaginationRequest request, Guid userId) =>
        new(request.SortBy, request.SortDirection, request.Page, request.PageSize, request.RequestStatus, userId);
}