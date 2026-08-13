namespace Boilerplate.Domain.TodoLists;

public sealed class Colour : ValueObject
{
    public static readonly Colour Red = new("#E05C4D");
    public static readonly Colour Orange = new("#D98B2B");
    public static readonly Colour Green = new("#4CAF50");
    public static readonly Colour Teal = new("#26A69A");
    public static readonly Colour Blue = new("#5C6BC0");
    public static readonly Colour Purple = new("#AB47BC");
    public static readonly Colour Grey = new("#78909C");

    private static readonly IReadOnlyCollection<Colour> SupportedColours =
        [Red, Orange, Green, Teal, Blue, Purple, Grey];

    public string Code { get; private set; }

    private Colour(string code) => Code = code;

    public static Colour From(string code)
    {
        var colour = new Colour(code);

        return SupportedColours.Contains(colour)
            ? colour
            : throw new ArgumentException(
                $"Colour \"{code}\" is not supported.",
                nameof(code));
    }

    public static bool IsSupported(string code) => SupportedColours.Contains(new Colour(code));

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }
}
