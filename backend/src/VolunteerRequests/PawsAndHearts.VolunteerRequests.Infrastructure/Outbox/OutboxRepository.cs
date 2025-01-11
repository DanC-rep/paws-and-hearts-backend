using System.Text.Json;
using PawsAndHearts.VolunteerRequests.Application.Interfaces;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Outbox;

public class OutboxRepository : IOutboxRepository
{
    private readonly WriteDbContext _dbContext;

    public OutboxRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add<T>(T message, CancellationToken cancellationToken = default)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccuredOnUtc = DateTime.Now,
            Type = typeof(T).FullName!,
            Payload = JsonSerializer.Serialize(message)
        };
        
        await _dbContext.AddAsync(outboxMessage, cancellationToken);
    }
}