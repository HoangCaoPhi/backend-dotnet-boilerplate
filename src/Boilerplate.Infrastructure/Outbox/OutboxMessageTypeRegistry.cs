using Boilerplate.Application.Common.IntegrationEvents;
using Boilerplate.Application.Common.Outbox;

namespace Boilerplate.Infrastructure.Outbox;

public static class OutboxMessageTypeRegistry
{
    private static readonly Lazy<Dictionary<string, Type>> TypesByName = new(Discover);

    public static void EnsureValid() => _ = TypesByName.Value;

    public static Type Resolve(string name)
        => TypesByName.Value.TryGetValue(name, out var type)
            ? type
            : throw new InvalidOperationException($"No outbox message type named '{name}'.");

    private static Dictionary<string, Type> Discover()
    {
        var messageTypes = typeof(IOutboxMessage).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IOutboxMessage)))
            .ToList();

        var duplicates = messageTypes
            .GroupBy(type => type.Name)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicates is not null)
        {
            throw new InvalidOperationException(
                "Outbox message names double as stored type names and exchange names, so they must be unique: "
                + string.Join(", ", duplicates.Select(type => type.FullName)));
        }

        var unroutable = messageTypes.FirstOrDefault(type =>
            !type.IsAssignableTo(typeof(IIntegrationEvent)) && !type.IsAssignableTo(typeof(IEventualCommand)));

        if (unroutable is not null)
        {
            throw new InvalidOperationException(
                $"Outbox message '{unroutable.FullName}' must be an {nameof(IIntegrationEvent)} or an {nameof(IEventualCommand)}; "
                + "the relay has nowhere to send it.");
        }

        return messageTypes.ToDictionary(type => type.Name);
    }
}
