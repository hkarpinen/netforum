using FluentAssertions;
using NETForum.Helpers;

namespace NETForum.UnitTests;

public class HtmlHelpersTests
{
    [Fact]
    public void StripAndTruncateHtml_WithShortText_ReturnsFullText()
    {
        var html = "<p>Short Text</p>";
        var maxLength = 100;
        
        var result = HtmlHelpers.StripAndTruncateHtml(html, maxLength);
        
        result.Should().Be("Short Text");
    }

    [Fact]
    public void StripAndTruncateHtml_WithLongText_TruncatesAndAddsEllipsis()
    {
        var html = "<p>This is a very long text that should be truncated</p>";
        var maxLength = 10;
        
        var result = HtmlHelpers.StripAndTruncateHtml(html, maxLength);

        result.Should().Be("This is a ...");
    }
}