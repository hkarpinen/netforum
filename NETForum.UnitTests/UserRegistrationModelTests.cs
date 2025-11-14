using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Account.Register;
using NETForum.Services;

namespace NETForum.UnitTests;

public class UserRegistrationModelTests
{
    private readonly Mock<IUserProfileService> _mockUserProfileService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IAuthenticationService> _mockAuthenticationService;
    private readonly IndexModel _pageModel;

    public UserRegistrationModelTests()
    {
        _mockUserProfileService = new Mock<IUserProfileService>();
        _mockUserService = new Mock<IUserService>();
        _mockAuthenticationService = new Mock<IAuthenticationService>();
        _pageModel = new IndexModel(
            _mockUserService.Object,
            _mockAuthenticationService.Object,
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
        var userCreateResult = Result.Success();
        var userLookupResult =
            Result<User>.Failure(new Error("User.NotFound", $"User with name {username} not found"));

        _mockUserService
            .Setup(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);

        _mockUserService
            .Setup(s => s.GetByUsernameAsync(username))
            .ReturnsAsync(userLookupResult);

        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _pageModel.ModelState["User.NotFound"].Errors[0].ErrorMessage.Should().Be($"User with name {username} not found");
        _mockUserService.Verify(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetByUsernameAsync(username), Times.Once);
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
        var userCreateResult = Result<User>.Failure(new Error("IdentityResultError", $"IdentityResultError"));
        
        _mockUserService
            .Setup(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _pageModel.ModelState["IdentityResultError"].Errors[0].ErrorMessage.Should().Be($"IdentityResultError");
        
        _mockUserService.Verify(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetByUsernameAsync(username), Times.Never);
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
        
        var userCreateResult = Result.Success();

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

        var userLookupResult = Result<User>.Success(expectedUser);

        var addUserProfileResult = Result<UserProfile>.Success(expectedUserProfile);
        
        _mockUserService
            .Setup(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(userRegistrationDto.Username))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.AddUserProfileAsync(userProfileDto))
            .ReturnsAsync(addUserProfileResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<RedirectToPageResult>();
        _mockUserService.Verify(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetByUsernameAsync(userRegistrationDto.Username), Times.Once);
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
        
        var userCreateResult = Result.Success();
        var userLookupResult = Result<User>.Success(expectedUser);
        var userProfileAddResult = Result<UserProfile>.Failure(new Error("UserProfile.AlreadyExists", $"User profile already exists"));
        
        _mockUserService
            .Setup(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(userRegistrationDto.Username))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.AddUserProfileAsync(userProfileDto))
            .ReturnsAsync(userProfileAddResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _pageModel.ModelState["UserProfile.AlreadyExists"].Errors[0].ErrorMessage.Should().Be($"User profile already exists");
        _mockUserService.Verify(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetByUsernameAsync(userRegistrationDto.Username), Times.Once);
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
        
        var userCreateResult = Result.Success();
        var userLookupResult = Result<User>.Success(expectedUser);
        var userProfileAddResult = Result<UserProfile>.Success(expectedUserProfile);
        
        _mockUserService
            .Setup(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto))
            .ReturnsAsync(userCreateResult);
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(userRegistrationDto.Username))
            .ReturnsAsync(userLookupResult);
        
        _mockUserProfileService
            .Setup(s => s.AddUserProfileAsync(userProfileDto))
            .ReturnsAsync(userProfileAddResult);

        _mockUserService
            .Setup(s => s.UpdateUserProfileImageAsync(expectedUser.Id, userProfileDto.ProfileImage))
            .ReturnsAsync(false);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState[""].Errors[0].ErrorMessage.Should().Be($"Could not add profile image");
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _mockUserService.Verify(s => s.CreateUserWithMemberRoleAsync(userRegistrationDto), Times.Once);
        _mockUserService.Verify(s => s.GetByUsernameAsync(userRegistrationDto.Username), Times.Once);
        _mockUserProfileService.Verify(s => s.AddUserProfileAsync(userProfileDto), Times.Once);
        _mockUserService.Verify(s => s.UpdateUserProfileImageAsync(expectedUser.Id, userProfileDto.ProfileImage));
        
    }

}