namespace PawsAndHearts.VolunteerRequests.Application.Interfaces;

public interface IOutboxRepository
{
    Task Add<T>(T message, CancellationToken cancellationToken = default);
}