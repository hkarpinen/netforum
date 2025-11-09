using FluentAssertions;
using NETForum.Helpers;

namespace NETForum.UnitTests;

public class DateHelperTests
{
    [Fact]
    public void ToRelativeDatetimeString_WithTodayDatetime_ReturnsTodayAtTime()
    {
        var dateTime = DateTime.Today.AddHours(15).AddMinutes(30);
        var result = DateHelpers.ToRelativeDatetimeString(dateTime);
        result.Should().StartWith("Today at");
        result.Should().Contain("3:30");
    }

    [Fact]
    public void ToRelativeDatetimeString_WithYesterdayDatetime_ReturnsYesterdayAtTime()
    {
        var dateTime = DateTime.Today.AddDays(-1).AddHours(15).AddMinutes(30);
        var result = DateHelpers.ToRelativeDatetimeString(dateTime);
        result.Should().StartWith("Yesterday at");
        result.Should().Contain("3:30");
    }
    
    [Fact]
    public void ToRelativeDatetimeString_WithOlderDatetime_ReturnsShortDateString()
    {
        var dateTime = DateTime.Today.AddMonths(-1).AddHours(15).AddMinutes(30);
        var result = DateHelpers.ToRelativeDatetimeString(dateTime);
        
        result.Should().StartWith(dateTime.ToShortDateString());
        result.Should().Contain(dateTime.ToShortTimeString());
    }
}