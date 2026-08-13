using Boilerplate.Application.Common.EventBus;
using Boilerplate.Application.TodoLists.IntegrationEvents.Events;
using Boilerplate.Domain.TodoLists.Events;

namespace Boilerplate.Application.TodoLists.EventHandlers;

public sealed class PublishIntegrationEventWhenTodoItemCompletedDomainEventHandler(IEventBus eventBus)
    : INotificationHandler<TodoItemCompletedDomainEvent>
{
    public async ValueTask Handle(
        TodoItemCompletedDomainEvent notification,
        CancellationToken cancellationToken)
        => await eventBus.PublishAsync(
            new TodoItemCompletedIntegrationEvent(
                notification.TodoListId,
                notification.TodoItemId),
            cancellationToken);
}
