using System.Text.Json;
using Boilerplate.Application.Common.Outbox;
using Boilerplate.Infrastructure.Persistence;
using Boilerplate.SharedKernel;

namespace Boilerplate.Infrastructure.Outbox;

public sealed class Outbox(
    ApplicationDbContext context,
    IIdGenerator idGenerator,
    ITimeProvider timeProvider) : IOutbox
{
    public void Add(IOutboxMessage message)
        => context.Set<OutboxMessage>().Add(OutboxMessage.Create(
            idGenerator.NewId(),
            message.GetType().Name,
            JsonSerializer.Serialize(
                message,
                message.GetType()),
            timeProvider.GetUtcNow()));
}
