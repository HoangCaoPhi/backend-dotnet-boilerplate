using Boilerplate.Domain.TodoLists;
using Boilerplate.Domain.TodoLists.Events;
using Shouldly;
using Xunit;

namespace Boilerplate.Domain.UnitTests.TodoLists;

public sealed class TodoListTests
{
    [Fact]
    public void Create_ValidTitle_SetsProperties()
    {
        // Arrange
        var id = Guid.CreateVersion7();

        // Act
        var todoList = TodoList.Create(
            id,
            "Groceries",
            Colour.Blue);

        // Assert
        todoList.Id.ShouldBe(id);
        todoList.Title.ShouldBe("Groceries");
        todoList.Colour.ShouldBe(Colour.Blue);
        todoList.Items.ShouldBeEmpty();
    }

    [Fact]
    public void Create_NoColour_DefaultsToGrey()
    {
        // Act
        var todoList = TodoList.Create(
            Guid.CreateVersion7(),
            "Groceries");

        // Assert
        todoList.Colour.ShouldBe(Colour.Grey);
    }

    [Fact]
    public void Create_WhitespaceTitle_ThrowsArgumentException()
        // Act & Assert
        => Should.Throw<ArgumentException>(() => TodoList.Create(
            Guid.CreateVersion7(),
            "   "));

    [Fact]
    public void AddItem_ValidInput_AddsItemToItems()
    {
        // Arrange
        var todoList = CreateTodoList();
        var itemId = Guid.CreateVersion7();

        // Act
        var item = todoList.AddItem(
            itemId,
            "Buy milk",
            null,
            PriorityLevel.Low,
            null,
            null);

        // Assert
        todoList.Items.ShouldHaveSingleItem();
        todoList.Items.ShouldContain(item);
        item.Id.ShouldBe(itemId);
        item.TodoListId.ShouldBe(todoList.Id);
    }

    [Fact]
    public void CompleteItem_ExistingItem_MarksDoneAndRaisesDomainEvent()
    {
        // Arrange
        var todoList = CreateTodoList();
        var item = todoList.AddItem(
            Guid.CreateVersion7(),
            "Buy milk",
            null,
            PriorityLevel.Low,
            null,
            null);

        // Act
        todoList.CompleteItem(item.Id);

        // Assert
        item.IsDone.ShouldBeTrue();
        var domainEvent = todoList.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<TodoItemCompletedDomainEvent>();
        domainEvent.TodoListId.ShouldBe(todoList.Id);
        domainEvent.TodoItemId.ShouldBe(item.Id);
    }

    [Fact]
    public void CompleteItem_UnknownItemId_ThrowsInvalidOperationException()
    {
        // Arrange
        var todoList = CreateTodoList();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => todoList.CompleteItem(Guid.CreateVersion7()));
    }

    [Fact]
    public void UpdateItemDetail_ExistingItem_UpdatesProperties()
    {
        // Arrange
        var todoList = CreateTodoList();
        var item = todoList.AddItem(
            Guid.CreateVersion7(),
            "Buy milk",
            null,
            PriorityLevel.Low,
            null,
            null);

        // Act
        todoList.UpdateItemDetail(
            item.Id,
            "Buy oat milk",
            "2 cartons",
            PriorityLevel.High,
            null);

        // Assert
        item.Title.ShouldBe("Buy oat milk");
        item.Note.ShouldBe("2 cartons");
        item.Priority.ShouldBe(PriorityLevel.High);
    }

    private static TodoList CreateTodoList()
        => TodoList.Create(
            Guid.CreateVersion7(),
            "Groceries",
            Colour.Blue);
}
