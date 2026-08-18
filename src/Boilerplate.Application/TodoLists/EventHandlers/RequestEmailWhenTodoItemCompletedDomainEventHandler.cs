using Boilerplate.Application.Common.DomainEvents;
using Boilerplate.Application.Common.Outbox;
using Boilerplate.Application.TodoLists.IntegrationEvents.Events;
using Boilerplate.Domain.TodoLists.Events;

namespace Boilerplate.Application.TodoLists.EventHandlers;

public sealed class RequestEmailWhenTodoItemCompletedDomainEventHandler(IOutbox outbox)
    : IDomainEventHandler<TodoItemCompletedDomainEvent>
{
    public ValueTask Handle(
        TodoItemCompletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        outbox.Add(new TodoItemCompletedEmailRequestedIntegrationEvent(
            notification.TodoListId,
            notification.TodoItemId));

        return ValueTask.CompletedTask;
    }
}
