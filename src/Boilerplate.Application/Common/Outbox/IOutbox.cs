namespace Boilerplate.Application.Common.Outbox;

public interface IOutbox
{
    void Add(IOutboxMessage message);
}
