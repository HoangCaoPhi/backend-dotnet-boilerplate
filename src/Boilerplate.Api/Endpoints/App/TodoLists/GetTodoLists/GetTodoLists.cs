using Boilerplate.Application.TodoLists.Queries.GetTodoLists;

namespace Boilerplate.Api.Endpoints.App.TodoLists.GetTodoLists;

public sealed class GetTodoLists : IAppEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet(
                "/api/todo-lists",
                Handle)
            .RequireAuthorization();

    private static async Task<IReadOnlyList<TodoListBriefDto>> Handle(
        ISender sender,
        CancellationToken cancellationToken)
        => await sender.Send(
            new GetTodoListsQuery(),
            cancellationToken);
}
