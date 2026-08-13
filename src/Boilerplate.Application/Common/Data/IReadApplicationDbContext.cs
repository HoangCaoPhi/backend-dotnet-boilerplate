using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Application.Common.Data;

public interface IReadApplicationDbContext
{
    IQueryable<TodoList> TodoLists { get; }

    IQueryable<TodoItem> TodoItems { get; }
}
