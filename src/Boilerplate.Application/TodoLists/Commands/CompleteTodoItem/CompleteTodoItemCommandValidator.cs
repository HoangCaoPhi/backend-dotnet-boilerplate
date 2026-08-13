using FluentValidation;

namespace Boilerplate.Application.TodoLists.Commands.CompleteTodoItem;

public sealed class CompleteTodoItemCommandValidator : AbstractValidator<CompleteTodoItemCommand>
{
    public CompleteTodoItemCommandValidator()
    {
        RuleFor(command => command.TodoListId).NotEmpty();
        RuleFor(command => command.TodoItemId).NotEmpty();
    }
}
