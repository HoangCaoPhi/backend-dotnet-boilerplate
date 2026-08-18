using System.Text.Json;
using Boilerplate.Application.Common.IntegrationEvents;
using Boilerplate.Application.Common.Outbox;
using Boilerplate.Infrastructure.Persistence;
using Boilerplate.SharedKernel;

namespace Boilerplate.Infrastructure.Outbox;

public sealed class Outbox(
    ApplicationDbContext context,
    IIdGenerator idGenerator,
    ITimeProvider timeProvider) : IOutbox
{
    public void Add(IIntegrationEvent integrationEvent)
        => context.Set<OutboxMessage>().Add(OutboxMessage.Create(
            idGenerator.NewId(),
            integrationEvent.GetType().Name,
            JsonSerializer.Serialize(
                integrationEvent,
                integrationEvent.GetType()),
            timeProvider.GetUtcNow()));
}
