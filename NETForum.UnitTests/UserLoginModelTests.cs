using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Pages.Account.Login;
using NETForum.Services;

namespace NETForum.UnitTests;

public class UserLoginModelTests
{
    private readonly Mock<IAuthenticationService> _mockAuthenticationService;
    private readonly IndexModel _pageModel;

    public UserLoginModelTests()
    {
        _mockAuthenticationService = new Mock<IAuthenticationService>();
        _pageModel = new IndexModel(_mockAuthenticationService.Object);
    }

    [Fact]
    public async Task OnPostAsync_WithSuccessfulSignIn_ShouldRedirect()
    {
        var userLoginDto = new UserLoginDto()
        {
            Username = "test",
            Password = "test",
            RememberMe = true
        };
        
        _pageModel.UserLoginDto = userLoginDto;
        
        _mockAuthenticationService
            .Setup(s => s.SignInAsync(userLoginDto))
            .ReturnsAsync(true);

        var result = await _pageModel.OnPostAsync();

        result.Should().BeOfType<RedirectToPageResult>();
        var redirectResult = result as RedirectToPageResult;
        redirectResult.PageName.Should().Be("/Index");
    }

    [Fact]
    public async Task OnPostAsync_WithUnsuccessfulSignIn_ShouldAddModelError()
    {
        var userLoginDto = new UserLoginDto()
        {
            Username = "test",
            Password = "test",
            RememberMe = false
        };
        _pageModel.UserLoginDto = userLoginDto;
        _mockAuthenticationService
            .Setup(s => s.SignInAsync(userLoginDto))
            .ReturnsAsync(false);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _pageModel.ModelState[""].Errors[0].ErrorMessage.Should().Be("Invalid username or password.");
    }
}