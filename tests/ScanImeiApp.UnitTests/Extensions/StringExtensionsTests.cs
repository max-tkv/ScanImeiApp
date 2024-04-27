using ScanImeiApp.Extensions;
using Xunit;

namespace ScanImeiApp.UnitTests.Extensions;

/// <summary>
/// Тесты для <see cref="StringExtensionsTests"/>.
/// </summary>
public class StringExtensionsTests : BaseUnitTests
{
    [Theory(DisplayName = "Удалены все двоеточии в начале строки.")]
    [InlineData("::ExampleText", "ExampleText")]
    [InlineData(":Example:Text:", "Example:Text:")]
    [InlineData("ExampleText:", "ExampleText:")]
    public void RemoveFirstCoupletChar_ShouldRemoveLeadingColons(string input, string expected)
    {
        // Act
        var result = input.DeleteFirstColonsChar();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory(DisplayName = "Удалены все двоеточии в конце строки.")]
    [InlineData("ExampleText::", "ExampleText")]
    [InlineData(":Example:Text:", ":Example:Text")]
    [InlineData("::ExampleText", "::ExampleText")]
    public void RemoveLastCoupletChar_ShouldRemoveTrailingColons(string input, string expected)
    {
        // Act
        var result = input.DeleteLastColonsChar();

        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory(DisplayName = "Заменены все двоеточия подряд на одно двоеточие.")]
    [InlineData("::", ":")]
    [InlineData("ExampleText::", "ExampleText:")]
    [InlineData(":Example::Text:", ":Example:Text:")]
    [InlineData(":Example:::::Text:::", ":Example:Text:")]
    [InlineData(":::ExampleText", ":ExampleText")]
    public void ReplaceMultipleColons_ShouldReplaceColons(string input, string expected)
    {
        // Act
        var result = input.ReplaceMultipleColons();

        // Assert
        Assert.Equal(expected, result);
    }
}