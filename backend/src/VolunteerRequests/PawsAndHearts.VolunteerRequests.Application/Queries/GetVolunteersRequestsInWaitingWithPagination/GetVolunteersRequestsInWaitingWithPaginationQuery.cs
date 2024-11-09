using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.VolunteerRequests.Contracts.Requests;

namespace PawsAndHearts.VolunteerRequests.Application.Queries.GetVolunteersRequestsInWaitingWithPagination;

public record GetVolunteersRequestsInWaitingWithPaginationQuery(
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize) : IQuery
{
    public static GetVolunteersRequestsInWaitingWithPaginationQuery Create(
        GetVolunteersRequestsInWaitingWithPaginationRequest request) =>
        new(request.SortBy, request.SortDirection, request.Page, request.PageSize);
}