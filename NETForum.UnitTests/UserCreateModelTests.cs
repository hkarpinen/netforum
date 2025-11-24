using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Users;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class UserCreateModelTests
{
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly CreateModel _pageModel;

    public UserCreateModelTests()
    {
        _mockRoleService = new Mock<IRoleService>();
        _mockUserService = new Mock<IUserService>();
        _pageModel = new CreateModel(
            _mockRoleService.Object,
            _mockUserService.Object
        );
    }

    [Fact]
    public async Task OnGetAsync_LoadsRoleSelectListItems()
    {
        var roleSelectListItems = new List<SelectListItem>()
        {
            new() { Text = "Role 1", Value = "1" },
            new() { Text = "Role 2", Value = "2" }
        };

        _mockRoleService
            .Setup(r => r.GetSelectItemsAsync())
            .ReturnsAsync(roleSelectListItems);
        
        await _pageModel.OnGetAsync();
        
        _pageModel.RoleSelectListItems.Should().HaveCount(2);
        _pageModel.RoleSelectListItems.Should().BeEquivalentTo(roleSelectListItems);
        
        _mockRoleService.Verify(r => r.GetSelectItemsAsync(), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_ReturnsPage()
    {
        _pageModel.ModelState.AddModelError("error", "error");
        
        var result = await _pageModel.OnPostAsync();
        result.Should().BeOfType<PageResult>();
    }
    
    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_DoesNotCreateUser()
    {
        _pageModel.ModelState.AddModelError("error", "error");
        
        await _pageModel.OnPostAsync();
        
        _mockUserService.Verify(u => u.CreateUserAsync(It.IsAny<CreateUserDto>()), Times.Never);
        _mockUserService.Verify(u => u.UpdateUserRolesAsync(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WithValidDtoAndRoles_CreatesUserAndRoles()
    {
        var createUserDto = new CreateUserDto()
        {
            Username = "test",
            Email = "test"
        };

        var selectedRoles = new List<string>()
        {
            "Role 1",
            "Role 2"
        };
        
        _pageModel.SelectedRoles = selectedRoles;
        _pageModel.CreateUserDto = createUserDto;
        
        var successResult = IdentityResult.Success;
        var createdUser = new User()
        {
            Id = 1,
            UserName = createUserDto.Username,
            Email = createUserDto.Email,
        };
        
        var serviceCreateResult = Result.Ok(createdUser);
        var serviceUserLookupResult = Result.Ok(createdUser);
        
        _mockUserService
            .Setup(u => u.CreateUserAsync(_pageModel.CreateUserDto))
            .ReturnsAsync(serviceCreateResult);
        
        _mockUserService
            .Setup(u => u.GetByUsernameAsync(createUserDto.Username))
            .ReturnsAsync(serviceUserLookupResult);
        
        _mockUserService
            .Setup(u => u.UpdateUserRolesAsync(createdUser.Id, It.IsAny<List<string>>()))
            .ReturnsAsync(Result.Ok());
        
        await _pageModel.OnPostAsync();
        
        _mockUserService.Verify(u => u.CreateUserAsync(createUserDto), Times.Once);
        _mockUserService.Verify(u => u.UpdateUserRolesAsync(createdUser.Id, selectedRoles), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserCreationFails_AddsErrorsToModelState()
    {
        var createUserDto = new CreateUserDto()
        {
            Username = "test",
            Email = "test"
        };
        
        _pageModel.CreateUserDto = createUserDto;
        
        var createUserResult = Result.Fail<User>("Error1");
        
        _mockUserService
            .Setup(u => u.CreateUserAsync(_pageModel.CreateUserDto))
            .ReturnsAsync(createUserResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _pageModel.ModelState["Error1"]?.Errors[0].ErrorMessage.Should().Be("Error1");
        
        _mockUserService.Verify(u => u.GetByUsernameAsync(It.IsAny<string>()), Times.Never);
        _mockUserService.Verify(u => u.UpdateUserRolesAsync(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsCreatedButNotFound_RolesAreNotAdded()
    {
        var createUserDto = new CreateUserDto()
        {
            Username = "test",
            Email = "test"
        };

        _pageModel.CreateUserDto = createUserDto;
        var successResult = IdentityResult.Success;


        var createdUser = new User()
        {
            Id = 1,
            UserName = createUserDto.Username,
            Email = createUserDto.Email,
        };
        var userCreateResult = Result.Ok(createdUser);
        
        _mockUserService
            .Setup(u => u.CreateUserAsync(createUserDto))
            .ReturnsAsync(userCreateResult);
        
        var userLookupResult = Result.Fail<User>("Error1");
        
        _mockUserService
            .Setup(u => u.GetByUsernameAsync(createUserDto.Username))
            .ReturnsAsync(userLookupResult);
        
        await _pageModel.OnPostAsync();
        
        _mockUserService.Verify(u => u.CreateUserAsync(createUserDto), Times.Once);
        _mockUserService.Verify(u => u.UpdateUserRolesAsync(It.IsAny<int>(), It.IsAny<List<string>>()), Times.Never);
    }
}