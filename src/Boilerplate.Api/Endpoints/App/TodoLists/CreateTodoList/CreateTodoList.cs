using Boilerplate.Application.TodoLists.Commands.CreateTodoList;
using Boilerplate.SharedKernel.Results;

namespace Boilerplate.Api.Endpoints.App.TodoLists.CreateTodoList;

public sealed class CreateTodoList : IAppEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost(
                "/api/todo-lists",
                Handle)
            .RequireAuthorization();

    private static async Task<Result<Guid>> Handle(
        CreateTodoListRequest request,
        HttpContext httpContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!httpContext.Request.Headers.TryGetValue(
                "Idempotency-Key",
                out var requestIdHeader)
            || !Guid.TryParse(
                requestIdHeader,
                out var requestId))
        {
            return Result<Guid>.Failure(new Error(
                "Idempotency.MissingKey",
                "Missing or invalid 'Idempotency-Key' header.",
                ErrorType.Validation));
        }

        var command = new CreateTodoListCommand(
            requestId,
            request.Title,
            request.ColourCode);

        return await sender.Send(
            command,
            cancellationToken);
    }
}
