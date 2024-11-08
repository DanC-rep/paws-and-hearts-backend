namespace PawsAndHearts.VolunteerRequests.Contracts.Requests;

public record GetVolunteersRequestsInWaitingWithPaginationRequest(
    string? SortBy,
    string? SortDirection,
    int Page, 
    int PageSize);