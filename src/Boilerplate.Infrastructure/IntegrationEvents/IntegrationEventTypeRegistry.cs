using Boilerplate.Application.Common.IntegrationEvents;

namespace Boilerplate.Infrastructure.IntegrationEvents;

public static class IntegrationEventTypeRegistry
{
    private static readonly Lazy<Dictionary<string, Type>> TypesByName = new(Discover);

    public static void EnsureValid() => _ = TypesByName.Value;

    public static Type Resolve(string name)
        => TypesByName.Value.TryGetValue(name, out var type)
            ? type
            : throw new InvalidOperationException($"No integration event type named '{name}'.");

    private static Dictionary<string, Type> Discover()
    {
        var types = typeof(IIntegrationEvent).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IIntegrationEvent)))
            .ToList();

        var duplicates = types
            .GroupBy(type => type.Name)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicates is not null)
        {
            throw new InvalidOperationException(
                $"Integration event names double as outbox type names and exchange names, so they must be unique: {string.Join(", ", duplicates.Select(type => type.FullName))}.");
        }

        return types.ToDictionary(type => type.Name);
    }
}
