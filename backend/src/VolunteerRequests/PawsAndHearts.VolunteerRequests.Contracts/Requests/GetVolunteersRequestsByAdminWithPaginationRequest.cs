namespace PawsAndHearts.VolunteerRequests.Contracts.Requests;

public record GetVolunteersRequestsByAdminWithPaginationRequest(
    string? SortBy,
    string? SortDirection,
    int Page, 
    int PageSize,
    string? RequestStatus);