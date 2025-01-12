namespace PawsAndHearts.VolunteerRequests.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public required string Type { get; set; } = default!;

    public required string Payload { get; set; } = default!;
    
    public required DateTime OccuredOnUtc { get; set; } 
    
    public DateTime? ProcessedOnUtc { get; set; }
    
    public string? Error { get; set; }
}