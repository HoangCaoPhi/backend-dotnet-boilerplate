using Boilerplate.Application.Common.IntegrationEvents;

namespace Boilerplate.Application.Common.Outbox;

public interface IOutbox
{
    void Add(IIntegrationEvent integrationEvent);
}
