using Boilerplate.Application.Common.Models;
using Boilerplate.Application.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Application.TodoLists.Queries.GetTodoItemsWithPagination;

public sealed class GetTodoItemsWithPaginationQueryHandler(IReadApplicationDbContext context)
    : IQueryHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    public ValueTask<PaginatedList<TodoItemBriefDto>> Handle(
        GetTodoItemsWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var items = context.TodoItems
            .Where(item => item.TodoListId == query.TodoListId)
            .OrderBy(item => item.Title)
            .Select(item => new TodoItemBriefDto(
                item.Id,
                item.Title,
                item.Priority,
                item.IsDone,
                item.Reminder));

        return new ValueTask<PaginatedList<TodoItemBriefDto>>(
            PaginatedList<TodoItemBriefDto>.CreateAsync(
                items,
                query.PageNumber,
                query.PageSize,
                cancellationToken));
    }
}
