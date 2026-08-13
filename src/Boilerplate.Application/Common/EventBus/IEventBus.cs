namespace Boilerplate.Application.Common.EventBus;

public interface IEventBus
{
    Task PublishAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken)
        where TIntegrationEvent : IIntegrationEvent;
}
