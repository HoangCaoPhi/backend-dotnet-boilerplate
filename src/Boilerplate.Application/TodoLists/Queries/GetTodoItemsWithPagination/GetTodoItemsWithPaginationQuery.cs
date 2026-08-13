using Boilerplate.Application.Common.Models;
using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Application.TodoLists.Queries.GetTodoItemsWithPagination;

public sealed record TodoItemBriefDto(
    Guid Id,
    string Title,
    PriorityLevel Priority,
    bool IsDone,
    DateTime? Reminder);

public sealed record GetTodoItemsWithPaginationQuery(
    Guid TodoListId,
    int PageNumber,
    int PageSize) : IQuery<PaginatedList<TodoItemBriefDto>>;
