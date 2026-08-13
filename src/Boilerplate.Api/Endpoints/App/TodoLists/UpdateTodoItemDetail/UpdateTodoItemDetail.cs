using Boilerplate.Application.TodoLists.Commands.UpdateTodoItemDetail;
using Boilerplate.SharedKernel.Results;

namespace Boilerplate.Api.Endpoints.App.TodoLists.UpdateTodoItemDetail;

public sealed class UpdateTodoItemDetail : IAppEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut(
                "/api/todo-lists/{todoListId:guid}/items/{todoItemId:guid}",
                Handle)
            .RequireAuthorization();

    private static async Task<Result> Handle(
        Guid todoListId,
        Guid todoItemId,
        UpdateTodoItemDetailRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTodoItemDetailCommand(
            todoListId,
            todoItemId,
            request.Title,
            request.Note,
            request.Priority,
            request.Reminder);

        return await sender.Send(
            command,
            cancellationToken);
    }
}
