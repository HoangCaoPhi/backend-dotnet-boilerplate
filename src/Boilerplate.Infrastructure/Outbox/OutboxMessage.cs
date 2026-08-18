namespace Boilerplate.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }

    public string Type { get; private set; } = null!;

    public string Content { get; private set; } = null!;

    public DateTimeOffset OccurredOn { get; private set; }

    public OutboxMessageStatus Status { get; private set; }

    public int Attempts { get; private set; }

    public DateTimeOffset? ProcessedOn { get; private set; }

    public string? Error { get; private set; }

    private OutboxMessage()
    {
    }

    public static OutboxMessage Create(
        Guid id,
        string type,
        string content,
        DateTimeOffset occurredOn)
        => new()
        {
            Id = id,
            Type = type,
            Content = content,
            OccurredOn = occurredOn,
            Status = OutboxMessageStatus.Pending,
        };

    public void MarkAttempted() => Attempts++;

    public void MarkPublished(DateTimeOffset processedOn)
    {
        Status = OutboxMessageStatus.Published;
        ProcessedOn = processedOn;
        Error = null;
    }

    public void MarkFailed(
        string error,
        int maxAttempts)
    {
        Status = Attempts >= maxAttempts ? OutboxMessageStatus.DeadLettered : OutboxMessageStatus.Failed;
        Error = error;
    }
}
