namespace Boilerplate.Domain.TodoLists;

public sealed class TodoItem : Entity
{
    public Guid TodoListId { get; private init; }

    public string Title { get; private set; }

    public string? Note { get; private set; }

    public PriorityLevel Priority { get; private set; }

    public DateTime? Reminder { get; private set; }

    public Guid? AssigneeUserId { get; private set; }

    public bool IsDone { get; private set; }

    private TodoItem(
        Guid id,
        Guid todoListId,
        string title,
        string? note,
        PriorityLevel priority,
        DateTime? reminder,
        Guid? assigneeUserId)
    {
        Id = id;
        TodoListId = todoListId;
        Title = title;
        Note = note;
        Priority = priority;
        Reminder = reminder;
        AssigneeUserId = assigneeUserId;
    }

    internal static TodoItem Create(
        Guid id,
        Guid todoListId,
        string title,
        string? note,
        PriorityLevel priority,
        DateTime? reminder,
        Guid? assigneeUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new TodoItem(
            id,
            todoListId,
            title,
            note,
            priority,
            reminder,
            assigneeUserId);
    }

    internal void UpdateDetail(
        string title,
        string? note,
        PriorityLevel priority,
        DateTime? reminder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Title = title;
        Note = note;
        Priority = priority;
        Reminder = reminder;
    }

    internal void Complete() => IsDone = true;
}
