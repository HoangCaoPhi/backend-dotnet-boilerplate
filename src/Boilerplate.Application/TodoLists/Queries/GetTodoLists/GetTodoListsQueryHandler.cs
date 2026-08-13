using Boilerplate.Application.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Application.TodoLists.Queries.GetTodoLists;

public sealed class GetTodoListsQueryHandler(IReadApplicationDbContext context)
    : IQueryHandler<GetTodoListsQuery, IReadOnlyList<TodoListBriefDto>>
{
    public async ValueTask<IReadOnlyList<TodoListBriefDto>> Handle(
        GetTodoListsQuery query,
        CancellationToken cancellationToken)
        => await context.TodoLists
            .Select(list => new TodoListBriefDto(
                list.Id,
                list.Title,
                list.Colour.Code,
                list.Items.Count(item => !item.IsDone)))
            .ToListAsync(cancellationToken);
}
