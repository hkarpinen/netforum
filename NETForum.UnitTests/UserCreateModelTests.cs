using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Users;
using NETForum.Services;

namespace NETForum.UnitTests;

public class UserCreateModelTests
{
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly CreateModel _systemUnderTest;

    public UserCreateModelTests()
    {
        _mockRoleService = new Mock<IRoleService>();
        _mockUserService = new Mock<IUserService>();
        _systemUnderTest = new CreateModel(
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
        
        await _systemUnderTest.OnGetAsync();
        
        _systemUnderTest.RoleSelectListItems.Should().HaveCount(2);
        _systemUnderTest.RoleSelectListItems.Should().BeEquivalentTo(roleSelectListItems);
        
        _mockRoleService.Verify(r => r.GetSelectItemsAsync(), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_ReturnsPage()
    {
        _systemUnderTest.ModelState.AddModelError("error", "error");
        
        var result = await _systemUnderTest.OnPostAsync();
        result.Should().BeOfType<PageResult>();
    }
    
    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_DoesNotCreateUser()
    {
        _systemUnderTest.ModelState.AddModelError("error", "error");
        
        await _systemUnderTest.OnPostAsync();
        
        _mockUserService.Verify(u => u.CreateUserAsync(It.IsAny<CreateUserDto>()), Times.Never);
        _mockUserService.Verify(u => u.UpdateUserRolesAsync(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Never);
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
        
        _systemUnderTest.SelectedRoles = selectedRoles;
        _systemUnderTest.CreateUserDto = createUserDto;
        
        var successResult = IdentityResult.Success;
        var createdUser = new User()
        {
            Id = 1,
            UserName = createUserDto.Username,
            Email = createUserDto.Email,
        };
        
        _mockUserService
            .Setup(u => u.CreateUserAsync(_systemUnderTest.CreateUserDto))
            .ReturnsAsync(successResult);
        
        _mockUserService
            .Setup(u => u.GetUserAsync(createUserDto.Username))
            .ReturnsAsync(createdUser);
        
        _mockUserService
            .Setup(u => u.UpdateUserRolesAsync(createdUser, It.IsAny<List<string>>()))
            .ReturnsAsync(successResult);
        
        await _systemUnderTest.OnPostAsync();
        
        _mockUserService.Verify(u => u.CreateUserAsync(createUserDto), Times.Once);
        _mockUserService.Verify(u => u.UpdateUserRolesAsync(createdUser, selectedRoles), Times.Once);
    }
}