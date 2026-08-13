using Boilerplate.Domain.TodoLists;

namespace Boilerplate.Infrastructure.Persistence.Repositories;

public sealed class TodoListRepository(ApplicationDbContext context) : ITodoListRepository
{
    public async Task AddAsync(
        TodoList aggregate,
        CancellationToken cancellationToken)
        => await context.Set<TodoList>().AddAsync(
            aggregate,
            cancellationToken);

    public async Task<TodoList?> GetByIdWithItemsAsync(
        Guid id,
        CancellationToken cancellationToken)
        => await context.Set<TodoList>()
            .Include(list => list.Items)
            .SingleOrDefaultAsync(
                list => list.Id == id,
                cancellationToken);
}
