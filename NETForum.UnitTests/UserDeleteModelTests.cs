using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NETForum.Constants;
using NETForum.Pages.Admin.Members;
using NETForum.Services;

namespace NETForum.UnitTests;

public class UserDeleteModelTests
{
    private readonly Mock<IUserService> _userService;
    private readonly DeleteModel _pageModel;

    public UserDeleteModelTests()
    {
        _userService = new Mock<IUserService>();
        _pageModel = new DeleteModel(_userService.Object);
    }

    [Fact]
    public async Task OnGetAsync_DeletesUserAndRedirectsToAdminUserManagementIndex()
    {
        var result = await _pageModel.OnGetAsync(1);
        result.Should().BeOfType<RedirectToPageResult>();
        var redirectToPageResult = result.As<RedirectToPageResult>();
        redirectToPageResult.PageName.Should().Be(PageRoutes.ManageMembers);
        
    }
}