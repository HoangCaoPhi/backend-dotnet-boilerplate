using Boilerplate.Application.Common.DomainEvents;
using Boilerplate.Application.Common.Outbox;
using Boilerplate.Application.TodoLists.Commands.SendTodoItemCompletedWebhook;
using Boilerplate.Domain.TodoLists.Events;

namespace Boilerplate.Application.TodoLists.EventHandlers;

public sealed class RequestWebhookWhenTodoItemCompletedDomainEventHandler(IOutbox outbox)
    : IDomainEventHandler<TodoItemCompletedDomainEvent>
{
    public ValueTask Handle(
        TodoItemCompletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        outbox.Add(new SendTodoItemCompletedWebhookCommand(
            notification.TodoListId,
            notification.TodoItemId,
            notification.AssigneeUserId));

        return ValueTask.CompletedTask;
    }
}
