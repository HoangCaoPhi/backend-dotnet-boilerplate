using Boilerplate.Application.Common.Models;
using Boilerplate.Application.TodoLists.Queries.GetTodoItemsWithPagination;

namespace Boilerplate.Api.Endpoints.App.TodoLists.GetTodoItemsWithPagination;

public sealed class GetTodoItemsWithPagination : IAppEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet(
                "/api/todo-lists/{todoListId:guid}/items",
                Handle)
            .RequireAuthorization();

    private static async Task<PaginatedList<TodoItemBriefDto>> Handle(
        Guid todoListId,
        int pageNumber,
        int pageSize,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetTodoItemsWithPaginationQuery(
            todoListId,
            pageNumber,
            pageSize);

        return await sender.Send(
            query,
            cancellationToken);
    }
}
