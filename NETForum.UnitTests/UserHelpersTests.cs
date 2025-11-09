using FluentAssertions;
using NETForum.Helpers;

namespace NETForum.UnitTests;

public class UserHelpersTests
{
    [Fact]
    public void GetInitials_WithUsername_ReturnsInitialsString()
    {
        var username = "testuser";

        var result = UserHelpers.GetInitials(username);

        result.Should().Be("TR");
    }
}