using Boilerplate.Application.TodoLists.Commands.CompleteTodoItem;
using Boilerplate.SharedKernel.Results;

namespace Boilerplate.Api.Endpoints.App.TodoLists.CompleteTodoItem;

public sealed class CompleteTodoItem : IAppEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost(
                "/api/todo-lists/{todoListId:guid}/items/{todoItemId:guid}/complete",
                Handle)
            .RequireAuthorization();

    private static async Task<Result> Handle(
        Guid todoListId,
        Guid todoItemId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CompleteTodoItemCommand(
            todoListId,
            todoItemId);

        return await sender.Send(
            command,
            cancellationToken);
    }
}
