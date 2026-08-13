using Boilerplate.Domain.TodoLists.Events;

namespace Boilerplate.Domain.TodoLists;

public sealed class TodoList : AggregateRoot
{
    private readonly List<TodoItem> _items = [];

    public string Title { get; private set; } = null!;

    public Colour Colour { get; private set; } = null!;

    public IReadOnlyCollection<TodoItem> Items => _items.AsReadOnly();

    private TodoList()
    {
    }

    private TodoList(
        Guid id,
        string title,
        Colour colour)
    {
        Id = id;
        Title = title;
        Colour = colour;
    }

    public static TodoList Create(
        Guid id,
        string title,
        Colour? colour = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new TodoList(
            id,
            title,
            colour ?? Colour.Grey);
    }

    public TodoItem AddItem(
        Guid itemId,
        string title,
        string? note,
        PriorityLevel priority,
        DateTime? reminder,
        Guid? assigneeUserId)
    {
        var item = TodoItem.Create(
            itemId,
            Id,
            title,
            note,
            priority,
            reminder,
            assigneeUserId);
        _items.Add(item);
        return item;
    }

    public void UpdateItemDetail(
        Guid itemId,
        string title,
        string? note,
        PriorityLevel priority,
        DateTime? reminder)
        => FindItem(itemId).UpdateDetail(
            title,
            note,
            priority,
            reminder);

    public void CompleteItem(Guid itemId)
    {
        var item = FindItem(itemId);
        item.Complete();
        AddDomainEvent(new TodoItemCompletedDomainEvent(
            Id,
            itemId));
    }

    private TodoItem FindItem(Guid itemId)
        => _items.SingleOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException($"Todo item '{itemId}' was not found in list '{Id}'.");
}
