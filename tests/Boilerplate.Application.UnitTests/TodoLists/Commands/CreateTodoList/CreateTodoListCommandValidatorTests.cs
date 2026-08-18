using Boilerplate.Application.TodoLists.Commands.CreateTodoList;
using Boilerplate.Domain.TodoLists;
using FluentValidation.TestHelper;
using Xunit;

namespace Boilerplate.Application.UnitTests.TodoLists.Commands.CreateTodoList;

public sealed class CreateTodoListCommandValidatorTests
{
    private readonly CreateTodoListCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyTitle_HasValidationErrorForTitle()
    {
        // Arrange
        var command = new CreateTodoListCommand(
            Guid.CreateVersion7(),
            string.Empty,
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Fact]
    public void Validate_UnsupportedColourCode_HasValidationErrorForColourCode()
    {
        // Arrange
        var command = new CreateTodoListCommand(
            Guid.CreateVersion7(),
            "Groceries",
            "#000000");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ColourCode);
    }

    [Fact]
    public void Validate_EmptyRequestId_HasValidationErrorForRequestId()
    {
        // Arrange
        var command = new CreateTodoListCommand(
            Guid.Empty,
            "Groceries",
            null);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.RequestId);
    }

    [Fact]
    public void Validate_ValidCommand_HasNoValidationErrors()
    {
        // Arrange
        var command = new CreateTodoListCommand(
            Guid.CreateVersion7(),
            "Groceries",
            Colour.Blue.Code);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
