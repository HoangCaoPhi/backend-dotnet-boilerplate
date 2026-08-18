namespace Boilerplate.Infrastructure.Outbox;

public enum OutboxMessageStatus
{
    Pending = 0,
    Published = 1,
    Failed = 2,
    DeadLettered = 3,
}
