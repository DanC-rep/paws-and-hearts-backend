using MediatR;
using PawsAndHearts.SharedKernel.Interfaces;

namespace PawsAndHearts.SharedKernel;

public static class MediatrExtensions
{
    public static async Task PublishDomainEvents<TId>(
        this IPublisher publisher,
        DomainEntity<TId> entity,
        CancellationToken cancellationToken = default) where TId : IComparable<TId>
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
        
        entity.ClearDomainEvents();
    }

    public static async Task PublishDomainEvent(
        this IPublisher publisher,
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await publisher.Publish(domainEvent, cancellationToken);
    }
}