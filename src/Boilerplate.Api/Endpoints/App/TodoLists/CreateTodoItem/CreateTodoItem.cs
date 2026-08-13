using Boilerplate.Application.TodoLists.Commands.CreateTodoItem;
using Boilerplate.SharedKernel.Results;

namespace Boilerplate.Api.Endpoints.App.TodoLists.CreateTodoItem;

public sealed class CreateTodoItem : IAppEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost(
                "/api/todo-lists/{todoListId:guid}/items",
                Handle)
            .RequireAuthorization();

    private static async Task<Result<Guid>> Handle(
        Guid todoListId,
        CreateTodoItemRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateTodoItemCommand(
            todoListId,
            request.Title,
            request.Note,
            request.Priority,
            request.Reminder,
            request.AssigneeUserId);

        return await sender.Send(
            command,
            cancellationToken);
    }
}
