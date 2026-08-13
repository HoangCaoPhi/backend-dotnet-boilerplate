using Boilerplate.Application.TodoLists.Queries.GetTodoLists;

namespace Boilerplate.Api.Endpoints.Integration.MobileApp.GetTodoLists;

public sealed class GetTodoLists : IIntegrationEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet(
                "/api/integration/mobile-app/todo-lists",
                Handle)
            .AddEndpointFilter(new IntegrationAuthFilter("MobileApp").InvokeAsync);

    private static async Task<IReadOnlyList<TodoListBriefDto>> Handle(
        ISender sender,
        CancellationToken cancellationToken)
        => await sender.Send(
            new GetTodoListsQuery(),
            cancellationToken);
}
