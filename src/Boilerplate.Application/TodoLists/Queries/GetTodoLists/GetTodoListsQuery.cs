namespace Boilerplate.Application.TodoLists.Queries.GetTodoLists;

public sealed record TodoListBriefDto(
    Guid Id,
    string Title,
    string ColourCode,
    int IncompleteItemCount);

public sealed record GetTodoListsQuery : IQuery<IReadOnlyList<TodoListBriefDto>>;
