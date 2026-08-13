using FluentValidation;

namespace Boilerplate.Application.TodoLists.Commands.CreateTodoItem;

public sealed class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(command => command.TodoListId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty();
    }
}
