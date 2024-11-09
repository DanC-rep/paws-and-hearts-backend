namespace PawsAndHearts.VolunteerRequests.Contracts.Requests;

public record GetVolunteerRequestsByUserWithPaginationRequest(
    string? SortBy,
    string? SortDirection,
    int Page, 
    int PageSize,
    string? RequestStatus);