using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Account.Register;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class UserRegistrationModelTests
{
    private readonly Mock<IUserProfileService> _mockUserProfileService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly IndexModel _pageModel;

    public UserRegistrationModelTests()
    {
        _mockUserProfileService = new Mock<IUserProfileService>();
        _mockUserService = new Mock<IUserService>();
        _pageModel = new IndexModel(
            _mockUserService.Object,
            _mockUserProfileService.Object
        );
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsCreatedButNotFound_AddsModelErrorAndReturnsPage()
    {
        var userRegistrationDto = new UserRegistrationDto()
        {
            Username = "test",
            Password = "test",
            ConfirmPassword = "test",
            Email = "test@email.com"
        };
        
        _pageModel.UserRegistrationDto = userRegistrationDto;

        var username = "test";
        var userCreateResult = Result.Ok();
        var userLookupResult = Result.Fail<User>("User could not be found");

        _mockUserService
            .Setup(s => s.RegisterUserAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);

        _mockUserService
            .Setup(s => s.GetUserAsync(username))
            .ReturnsAsync(userLookupResult);

        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _mockUserService.Verify(s => s.RegisterUserAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetUserAsync(username), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserCreationFails_AddModelErrorAndReturnsPage()
    {
        var userRegistrationDto = new UserRegistrationDto()
        {
            Username = "test",
            Password = "test",
            ConfirmPassword = "test",
            Email = "test@email.com"
        };
        
        _pageModel.UserRegistrationDto = userRegistrationDto;
        
        var username = "test";
        var userCreateResult = Result.Fail("User could not be created");
        
        _mockUserService
            .Setup(s => s.RegisterUserAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        
        _mockUserService.Verify(s => s.RegisterUserAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetUserAsync(username), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserCreationSucceedsAndUserProfileDoesNotExists_CreatesUserProfile()
    {
        var userRegistrationDto = new UserRegistrationDto()
        {
            Username = "test",
            Password = "test",
            ConfirmPassword = "test",
            Email = "test@email.com"
        };

        var userProfileDto = new CreateUserProfileDto()
        {
            UserId = 1,
            Bio = "test",
            Location = "test"
        };
        
        _pageModel.UserRegistrationDto = userRegistrationDto;
        _pageModel.CreateUserProfileDto = userProfileDto;
        
        var userCreateResult = Result.Ok();

        var expectedUser = new User()
        {
            Id = 1,
            Email = "test@email.com",
            UserName = "test"
        };

        var expectedUserProfile = new UserProfile()
        {
            Id = 1,
            UserId = 1,
            Bio = "test",
            Location = "test"
        };

        var userLookupResult = Result.Ok(expectedUser);

        var addUserProfileResult = Result.Ok(expectedUserProfile);
        
        _mockUserService
            .Setup(s => s.RegisterUserAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        _mockUserService
            .Setup(s => s.GetUserAsync(userRegistrationDto.Username))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.AddUserProfileAsync(userProfileDto))
            .ReturnsAsync(addUserProfileResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<RedirectToPageResult>();
        _mockUserService.Verify(s => s.RegisterUserAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetUserAsync(userRegistrationDto.Username), Times.Once);
        _mockUserProfileService.Verify(s => s.AddUserProfileAsync(userProfileDto), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsCreatedAndCreatingUserProfileFails_ShouldAddModelErrorAndReturnPage()
    {
        var userRegistrationDto = new UserRegistrationDto()
        {
            Username = "test",
            Password = "test",
            ConfirmPassword = "test",
            Email = "test@test.com"
        };

        var expectedUser = new User()
        {
            Id = 1,
            UserName = "test",
            Email = "test@test.com"
        };

        var userProfileDto = new CreateUserProfileDto()
        {
            UserId = 1,
            Bio = "test",
            Location = "test"
        };
        
        _pageModel.UserRegistrationDto = userRegistrationDto;
        _pageModel.CreateUserProfileDto = userProfileDto;

        var userCreateResult = Result.Ok();
        var userLookupResult = Result.Ok(expectedUser);
        var userProfileAddResult = Result.Fail<UserProfile>("User profile could not be created");
        
        _mockUserService
            .Setup(s => s.RegisterUserAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        _mockUserService
            .Setup(s => s.GetUserAsync(userRegistrationDto.Username))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.AddUserProfileAsync(userProfileDto))
            .ReturnsAsync(userProfileAddResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _mockUserService.Verify(s => s.RegisterUserAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetUserAsync(userRegistrationDto.Username), Times.Once);
        _mockUserProfileService.Verify(s => s.AddUserProfileAsync(userProfileDto), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserSuccessfullyRegistersAndAddingProfileImageFails_ShouldAddModelErrorAndReturnPage()
    {
        var userRegistrationDto = new UserRegistrationDto()
        {
            Username = "test",
            Password = "test",
            ConfirmPassword = "test",
            Email = "test@test.com"
        };

        var expectedUser = new User()
        {
            Id = 1,
            UserName = "test",
            Email = "test@test.com",
        };

        var userProfileDto = new CreateUserProfileDto()
        {
            UserId = 1,
            Bio = "test",
            Location = "test",
            ProfileImage = new Mock<IFormFile>().Object
        };

        var expectedUserProfile = new UserProfile()
        {
            Id = 1,
            UserId = 1,
            Bio = "test",
            Location = "test"
        };
        
        _pageModel.UserRegistrationDto = userRegistrationDto;
        _pageModel.CreateUserProfileDto = userProfileDto;
        
        var userCreateResult = Result.Ok();
        var userLookupResult = Result.Ok(expectedUser);
        var userProfileAddResult = Result.Ok(expectedUserProfile);
        
        _mockUserService
            .Setup(s => s.RegisterUserAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        _mockUserService
            .Setup(s => s.GetUserAsync(userRegistrationDto.Username))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.AddUserProfileAsync(userProfileDto))
            .ReturnsAsync(userProfileAddResult);

        _mockUserService
            .Setup(s => s.UpdateUserProfileImageAsync(expectedUser.Id, userProfileDto.ProfileImage))
            .ReturnsAsync(Result.Fail("Could not add profile image"));
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _mockUserService.Verify(s => s.RegisterUserAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetUserAsync(userRegistrationDto.Username), Times.Once);
        _mockUserProfileService.Verify(s => s.AddUserProfileAsync(userProfileDto), Times.Once);
        _mockUserService.Verify(s => s.UpdateUserProfileImageAsync(expectedUser.Id, userProfileDto.ProfileImage));
        
    }

}