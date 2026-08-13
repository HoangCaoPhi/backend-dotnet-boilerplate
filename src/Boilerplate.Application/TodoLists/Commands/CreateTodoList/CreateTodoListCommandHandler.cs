using Boilerplate.Domain.TodoLists;
using Boilerplate.SharedKernel;

namespace Boilerplate.Application.TodoLists.Commands.CreateTodoList;

public sealed class CreateTodoListCommandHandler(
    ITodoListRepository todoListRepository,
    IIdGenerator idGenerator) : ICommandHandler<CreateTodoListCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateTodoListCommand command,
        CancellationToken cancellationToken)
    {
        var colour = command.ColourCode is null
            ? Colour.Grey
            : Colour.From(command.ColourCode);

        var todoList = TodoList.Create(
            idGenerator.NewId(),
            command.Title,
            colour);

        await todoListRepository.AddAsync(
            todoList,
            cancellationToken);

        return Result<Guid>.Success(todoList.Id);
    }
}
