namespace Boilerplate.Api.Endpoints.App.TodoLists.CreateTodoList;

public sealed record CreateTodoListRequest(
    string Title,
    string? ColourCode);
