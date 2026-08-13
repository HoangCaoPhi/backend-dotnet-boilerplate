using Boilerplate.Application.Common.Idempotency;

namespace Boilerplate.Application.TodoLists.Commands.CreateTodoList;

public sealed record CreateTodoListCommand(
    Guid RequestId,
    string Title,
    string? ColourCode) : ICommand<Result<Guid>>, IIdempotentCommand;
