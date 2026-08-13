namespace Boilerplate.Infrastructure.Outbox;
 
public interface IIntegrationEventPublisher
{
    Task PublishAsync(
        string eventType,
        string content,
        CancellationToken cancellationToken);
}
