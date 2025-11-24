using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Account.Profile;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class UserProfileEditModelTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IUserProfileService> _mockUserProfileService;
    private readonly EditModel _pageModel;

    public UserProfileEditModelTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockUserProfileService = new Mock<IUserProfileService>();
        _pageModel = new EditModel(
            _mockUserService.Object,
            _mockUserProfileService.Object
        );
        _pageModel.PageContext = new PageContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = GetClaimsPrincipal()
            }
        };
    }

    private ClaimsPrincipal GetClaimsPrincipal()
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "testuser")
        }, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return claimsPrincipal;
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsNotFound_ShouldReturnNotFoundResult()
    {
        var userLookupResult = Result.Fail<User>("User not found");
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(userLookupResult);
        
        var result = await _pageModel.OnGetAsync();
        
        result.Should().BeOfType<NotFoundResult>();
        _mockUserService.Verify(s => s.GetByUsernameAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsFoundAndProfileIsNotFound_ShouldReturnNotFoundResult()
    {
        var expectedUser = new User()
        {
            Id = 1,
            Email = "test@test.com",
            UserName = "test"
        };
        
        var userLookupResult = Result.Ok(expectedUser);
        var userProfileLookupResult = Result.Fail<EditUserProfileDto>("User profile not found.");
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(userLookupResult);

        _mockUserProfileService
            .Setup(s => s.GetUserProfileForEditAsync(expectedUser.Id))
            .ReturnsAsync(userProfileLookupResult);
        
        var result = await _pageModel.OnGetAsync();
        
        _mockUserService.Verify(s => s.GetByUsernameAsync(It.IsAny<string>()), Times.Once);
        _mockUserProfileService.Verify(s => s.GetUserProfileForEditAsync(expectedUser.Id), Times.Once);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsFoundAndProfileIsFound_ShouldPopulateDto()
    {
        var expectedUser = new User()
        {
            Id = 1,
            Email = "test@test.com",
            UserName = "test"
        };

        var expectedEditUserProfileDto = new EditUserProfileDto()
        {
            Bio = "test",
            Location = "test"
        };
        
        var userLookupResult = Result.Ok(expectedUser);
        var userProfileLookupResult = Result.Ok(expectedEditUserProfileDto);
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.GetUserProfileForEditAsync(expectedUser.Id))
            .ReturnsAsync(userProfileLookupResult);
        
        var result = await _pageModel.OnGetAsync();
        
        _mockUserService.Verify(s => s.GetByUsernameAsync(It.IsAny<string>()), Times.Once);
        _mockUserProfileService.Verify(s => s.GetUserProfileForEditAsync(expectedUser.Id), Times.Once);
        _pageModel.EditUserProfileDto.Should().BeEquivalentTo(expectedEditUserProfileDto);
        result.Should().BeOfType<PageResult>();
        
    }
    
    [Fact]
    public async Task OnPostAsync_WhenModelStateIsInvalid_ShouldReturnPageResult()
    {
        _pageModel.ModelState.AddModelError("error", "error");
        
        var result = await _pageModel.OnPostAsync()
            ;
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsNotFound_ShouldReturnNotFoundResult()
    {
        var userLookupResult = Result.Fail<User>("User not found");
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(userLookupResult);
        
        var result = await _pageModel.OnPostAsync();
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsFound_ShouldUpdateUserProfileAndRedirect()
    {
        var expectedUser = new User()
        {
            Id = 1,
            UserName = "test",
            Email = "test@test.com"
        };

        var editUserProfileDto = new EditUserProfileDto()
        {
            Bio = "test",
            Location = "test"
        };
        
        _pageModel.EditUserProfileDto = editUserProfileDto;
        
        var userLookupResult = Result.Ok(expectedUser);
        var userProfileUpdateResult = Result.Ok();
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(It.IsAny<string>()))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.UpdateUserProfileAsync(expectedUser.Id, editUserProfileDto))
            .ReturnsAsync(userProfileUpdateResult);
        
        var result = await _pageModel.OnPostAsync();
        
        _mockUserService.Verify(s => s.GetByUsernameAsync(It.IsAny<string>()), Times.Once);
        _mockUserProfileService.Verify(s => s.UpdateUserProfileAsync(expectedUser.Id, editUserProfileDto), Times.Once);
        result.Should().BeOfType<RedirectToPageResult>();
    }
}