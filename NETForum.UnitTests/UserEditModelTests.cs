using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Services;
using FluentResults;
using NETForum.Constants;
using NETForum.Errors;
using NETForum.Pages.Admin.Members;

namespace NETForum.UnitTests;

public class UserEditModelTests
{
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IUserRoleService> _mockUserRoleService;
    private readonly EditModel _pageModel;

    public UserEditModelTests()
    {
        _mockRoleService = new Mock<IRoleService>();
        _mockUserService = new Mock<IUserService>();
        _mockUserRoleService = new Mock<IUserRoleService>();
        _pageModel = new EditModel(
            _mockRoleService.Object,
            _mockUserService.Object,
            _mockUserRoleService.Object
        );
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsNotFound_ReturnsNotFoundResult()
    {
        _mockUserService
            .Setup(u => u.GetUserForEditAsync(1))
            .ReturnsAsync(() => Result.Fail<EditUserDto>("Could not find user"));

        var result = await _pageModel.OnGetAsync(1);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsFound_ShouldPopulateFormAndDropdowns()
    {
        var editUserDto = new EditUserDto()
        {
            Username = "test",
            Email = "test"
        };
        
        _mockUserService
            .Setup(u => u.GetUserForEditAsync(1))
            .ReturnsAsync(editUserDto);

        var allRoles = new List<SelectListItem>()
        {
            new() { Text = "Role1", Value = "Role1" },
            new() { Text = "Role2", Value = "Role2" }
        };
        
        _mockRoleService
            .Setup(r => r.GetSelectItemsAsync())
            .ReturnsAsync(allRoles);

        var selectedRoles = new List<string>()
        {
            "Role1"
        };
        
        _mockUserRoleService
            .Setup(ur => ur.GetUserRoleNamesAsync(1))
            .ReturnsAsync(selectedRoles);
        
        await _pageModel.OnGetAsync(1);
        
        _pageModel.EditUserDto.Should().BeEquivalentTo(editUserDto);
        _pageModel.AllRoles.Should().BeEquivalentTo(allRoles);
        _pageModel.SelectedRoles.Should().BeEquivalentTo(selectedRoles);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserIsFound_ShouldReturnPageResult()
    {
        var editUserDto = new EditUserDto()
        {
            Username = "test",
            Email = "test"
        };
        
        _mockUserService
            .Setup(u => u.GetUserForEditAsync(1))
            .ReturnsAsync(editUserDto);
            
        
        var result = await _pageModel.OnGetAsync(1);
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsNotFound_ReturnsNotFoundResult()
    {
        var roles = new List<string>()
        {
            "Role1"
        };

        _pageModel.SelectedRoles = roles;

        var updateUserRolesResult = Result.Fail(UserErrors.NotFound(1));
        
        _mockUserService
            .Setup(u => u.UpdateUserRolesAsync(1, roles))
            .ReturnsAsync(updateUserRolesResult);
        
        var result = await _pageModel.OnPostAsync(1);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnPostAsync_WhenUserIsFound_ShouldUpdateRolesAndRedirect()
    {
        _mockUserService
            .Setup(u => u.UpdateUserRolesAsync(1, It.IsAny<List<string>>()))
            .ReturnsAsync(Result.Ok());
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<RedirectToPageResult>();
        var redirect = result as RedirectToPageResult;
        redirect.PageName.Should().Be(PageRoutes.ManageMembers);
        
        _mockUserService.Verify(u => u.UpdateUserRolesAsync(1, It.IsAny<List<string>>()), Times.Once);
    }
    
}