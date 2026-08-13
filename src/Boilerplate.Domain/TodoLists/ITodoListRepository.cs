namespace Boilerplate.Domain.TodoLists;

public interface ITodoListRepository : IRepository<TodoList>
{
    Task<TodoList?> GetByIdWithItemsAsync(
        Guid id,
        CancellationToken cancellationToken);
}
