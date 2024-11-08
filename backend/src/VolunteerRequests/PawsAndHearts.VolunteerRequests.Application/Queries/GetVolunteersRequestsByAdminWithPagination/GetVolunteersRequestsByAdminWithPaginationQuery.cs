using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Contracts.Requests;

namespace PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteersRequestsByAdminWithPagination;

public record GetVolunteersRequestsByAdminWithPaginationQuery(
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize,
    string? RequestStatus,
    Guid AdminId) : IQuery
{
    public static GetVolunteersRequestsByAdminWithPaginationQuery Create(
        GetVolunteersRequestsByAdminWithPaginationRequest request, Guid adminId) =>
        new(request.SortBy, request.SortDirection, request.Page, request.PageSize, request.RequestStatus, adminId);
}