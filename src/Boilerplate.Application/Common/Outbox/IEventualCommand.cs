using Mediator;

namespace Boilerplate.Application.Common.Outbox;

public interface IEventualCommand : IOutboxMessage, ICommand;
