using Boilerplate.Application.Common.Data;
using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Infrastructure.Persistence;

public sealed class ReadApplicationDbContext(ApplicationDbContext context) : IReadApplicationDbContext
{
    public IQueryable<TodoList> TodoLists => context.Set<TodoList>().AsNoTracking();

    public IQueryable<TodoItem> TodoItems => context.Set<TodoItem>().AsNoTracking();
}
