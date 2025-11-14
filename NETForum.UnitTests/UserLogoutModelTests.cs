using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NETForum.Pages.Account.Logout;
using NETForum.Services;

namespace NETForum.UnitTests;

public class UserLogoutModelTests
{
    private readonly Mock<IAuthenticationService> _mockAuthenticationService;
    private readonly LogoutModel  _pageModel;

    public UserLogoutModelTests()
    {
        _mockAuthenticationService = new Mock<IAuthenticationService>();
        _pageModel = new LogoutModel(_mockAuthenticationService.Object);
    }

    [Fact]
    public async Task OnGetAsync_ShouldLogoutAndRedirect()
    {
        var result = await _pageModel.OnGetAsync();
        
        _mockAuthenticationService.Verify(s => s.SignOutAsync(), Times.Once);
        result.Should().BeOfType<RedirectToPageResult>();
        var redirectResult = result as RedirectToPageResult;
        redirectResult.PageName.Should().Be("/Index");
    }
    
}