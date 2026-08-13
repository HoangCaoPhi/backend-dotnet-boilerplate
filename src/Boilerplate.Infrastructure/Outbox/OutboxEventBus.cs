using System.Text.Json;
using Boilerplate.Application.Common.EventBus;
using Boilerplate.Infrastructure.Persistence;
using Boilerplate.SharedKernel;

namespace Boilerplate.Infrastructure.Outbox;

// No SaveChangesAsync here on purpose: it rides the SaveChanges already in flight.
public sealed class OutboxEventBus(
    ApplicationDbContext context,
    IIdGenerator idGenerator,
    ITimeProvider timeProvider) : IEventBus
{
    public async Task PublishAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
        where TIntegrationEvent : IIntegrationEvent
    {
        var message = OutboxMessage.Create(
            idGenerator.NewId(),
            typeof(TIntegrationEvent).AssemblyQualifiedName!,
            JsonSerializer.Serialize(integrationEvent),
            timeProvider.GetUtcNow());

        await context.Set<OutboxMessage>().AddAsync(
            message,
            cancellationToken);
    }
}
