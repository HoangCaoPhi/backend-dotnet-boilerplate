using Mediator;
using Boilerplate.Domain.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Boilerplate.Infrastructure.Persistence.Interceptors;

public sealed class DispatchDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(
            eventData.Context,
            cancellationToken);

        return await base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(
        DbContext? context,
        CancellationToken cancellationToken)
    {
        if (context is null)
        {
            return;
        }

        while (true)
        {
            var domainEvents = CollectAndClearDomainEvents(context);

            if (domainEvents.Count == 0)
            {
                return;
            }

            foreach (var domainEvent in domainEvents)
            {
                await publisher.Publish(
                    domainEvent,
                    cancellationToken);
            }
        }
    }

    private static IReadOnlyCollection<IDomainEvent> CollectAndClearDomainEvents(DbContext context)
    {
        var aggregates = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        aggregates.ForEach(aggregate => aggregate.ClearDomainEvents());

        return domainEvents;
    }
}
