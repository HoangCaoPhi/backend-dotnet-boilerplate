namespace Boilerplate.Application.Common.IntegrationEvents;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(
        IIntegrationEvent integrationEvent,
        Guid messageId,
        DateTimeOffset occurredOn,
        CancellationToken cancellationToken);
}
