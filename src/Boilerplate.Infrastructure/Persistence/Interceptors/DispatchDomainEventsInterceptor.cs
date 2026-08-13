using Boilerplate.Domain.Common;
using Mediator;
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
            var aggregates = context.ChangeTracker
                .Entries<AggregateRoot>()
                .Select(entry => entry.Entity)
                .Where(aggregate => aggregate.DomainEvents.Count > 0)
                .ToList();

            if (aggregates.Count == 0)
            {
                return;
            }

            var domainEvents = aggregates
                .SelectMany(aggregate => aggregate.DomainEvents)
                .ToList();

            aggregates.ForEach(aggregate => aggregate.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await publisher.Publish(
                    domainEvent,
                    cancellationToken);
            }
        }
    }
}
