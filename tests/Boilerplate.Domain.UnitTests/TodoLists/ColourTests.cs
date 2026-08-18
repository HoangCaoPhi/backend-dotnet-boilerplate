using Boilerplate.Domain.TodoLists;
using Shouldly;
using Xunit;

namespace Boilerplate.Domain.UnitTests.TodoLists;

public sealed class ColourTests
{
    [Fact]
    public void From_SupportedCode_ReturnsColourWithThatCode()
    {
        var colour = Colour.From(Colour.Blue.Code);

        colour.Code.ShouldBe(Colour.Blue.Code);
    }

    [Fact]
    public void From_UnsupportedCode_ThrowsArgumentException()
        => Should.Throw<ArgumentException>(() => Colour.From("#000000"));

    [Fact]
    public void IsSupported_SupportedCode_ReturnsTrue()
        => Colour.IsSupported(Colour.Green.Code).ShouldBeTrue();

    [Fact]
    public void IsSupported_UnsupportedCode_ReturnsFalse()
        => Colour.IsSupported("#000000").ShouldBeFalse();
}
