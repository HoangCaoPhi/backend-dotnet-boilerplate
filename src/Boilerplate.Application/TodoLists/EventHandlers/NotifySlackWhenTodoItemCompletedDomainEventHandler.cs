using Boilerplate.Application.Common.ExternalClients.Slack;
using Boilerplate.Domain.TodoLists.Events;

namespace Boilerplate.Application.TodoLists.EventHandlers;

public sealed class NotifySlackWhenTodoItemCompletedDomainEventHandler(ISlackClient slackClient)
    : INotificationHandler<TodoItemCompletedDomainEvent>
{
    public async ValueTask Handle(
        TodoItemCompletedDomainEvent notification,
        CancellationToken cancellationToken)
        => await slackClient.NotifyAsync(
            $"Todo item '{notification.TodoItemId}' in list '{notification.TodoListId}' completed.",
            cancellationToken);
}
