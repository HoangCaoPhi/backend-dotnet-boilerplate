using Boilerplate.Application.TodoLists.Queries.GetTodoLists;

namespace Boilerplate.Api.Endpoints.Integration.ReportingService.GetTodoLists;

public sealed class GetTodoLists : IIntegrationEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet(
                "/api/integration/reporting-service/todo-lists",
                Handle)
            .AddEndpointFilter(new IntegrationAuthFilter("ReportingService").InvokeAsync);

    private static async Task<IReadOnlyList<TodoListBriefDto>> Handle(
        ISender sender,
        CancellationToken cancellationToken)
        => await sender.Send(
            new GetTodoListsQuery(),
            cancellationToken);
}
