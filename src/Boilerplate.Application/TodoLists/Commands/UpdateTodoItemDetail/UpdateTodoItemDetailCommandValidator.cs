using FluentValidation;

namespace Boilerplate.Application.TodoLists.Commands.UpdateTodoItemDetail;

public sealed class UpdateTodoItemDetailCommandValidator : AbstractValidator<UpdateTodoItemDetailCommand>
{
    public UpdateTodoItemDetailCommandValidator()
    {
        RuleFor(command => command.TodoListId).NotEmpty();
        RuleFor(command => command.TodoItemId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty();
    }
}
