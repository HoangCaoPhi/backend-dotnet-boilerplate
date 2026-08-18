using Boilerplate.Domain.Common;

namespace Boilerplate.Application.Common.DomainEvents;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;
