using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Pages.Forums;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class ForumEditModelTests
{
    private readonly Mock<IForumService> _mockForumService;
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly EditModel _pageModel;

    public ForumEditModelTests()
    {
        _mockForumService = new Mock<IForumService>();
        _mockCategoryService = new Mock<ICategoryService>();
        _pageModel = new EditModel(_mockForumService.Object, categoryService: _mockCategoryService.Object);
    }

    [Fact]
    public async Task OnGetAsync_WhenForumIsNotFound_ReturnsNotFoundResult()
    {
        var forumLookupResult = Result.Fail<EditForumDto>("Forum not found");
        
        _mockForumService
            .Setup(s => s.GetForumForEditAsync(1))
            .ReturnsAsync(forumLookupResult);
        
        var result = await _pageModel.OnGetAsync(1);

        result.Should().BeOfType<NotFoundResult>();
        _mockForumService.Verify(s => s.GetForumForEditAsync(1), Times.Once);
        _mockForumService.Verify(s => s.GetForumSelectListItemsAsync(), Times.Never);
        _mockCategoryService.Verify(s => s.GetCategorySelectListItemsAsync(), Times.Never);
    }
    
    [Fact]
    public async Task OnGetAsync_WhenForumIsFound_PopulatesSelectListItemsAndReturnsPage()
    {
        var editForumDto = new EditForumDto()
        {
            Id = 1,
            CategoryId = 1,
            Name = "test",
            Description = "test"
        };

        var parentForumSelectListItems = new List<SelectListItem>()
        {
            new() { Value = "1", Text = "1" },
            new() { Value = "2", Text = "2" }
        };

        var categorySelectListItems = new List<SelectListItem>()
        {
            new() { Value = "1", Text = "1" },
            new() { Value = "2", Text = "2" }
        };

        var forumLookupResult = Result.Ok(editForumDto);
        
        _mockForumService
            .Setup(s => s.GetForumForEditAsync(1))
            .ReturnsAsync(forumLookupResult);
        
        _mockForumService
            .Setup(s => s.GetForumSelectListItemsAsync())
            .ReturnsAsync(parentForumSelectListItems);
        
        _mockCategoryService
            .Setup(s => s.GetCategorySelectListItemsAsync())
            .ReturnsAsync(categorySelectListItems);
        
        var result = await _pageModel.OnGetAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.EditForumDto.Should().BeEquivalentTo(editForumDto);
        _pageModel.CategorySelectListItems.Should().BeEquivalentTo(categorySelectListItems);
        _pageModel.ParentForumSelectListItems.Should().BeEquivalentTo(parentForumSelectListItems);
    }

    [Fact]
    public async Task OnPostAsync_WhenForumIsNotFound_ReturnsNotFoundResult()
    {
        var forumLookupResult = Result.Fail<EditForumDto>("Forum not found");
        
        _mockForumService
            .Setup(s=>s.GetForumForEditAsync(1))
            .ReturnsAsync(forumLookupResult);
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<NotFoundResult>();
        _mockForumService.Verify(s => s.GetForumForEditAsync(1), Times.Once);
        _mockCategoryService.Verify(s => s.GetCategorySelectListItemsAsync(), Times.Never);
        _mockForumService.Verify(s => s.GetForumSelectListItemsAsync(), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateIsInvalid_ReturnsPageResult()
    {
        var expectedForumEditDto = new EditForumDto()
        {
            Id = 1,
            Name = "test",
            Description = "test"
        };
        
        var forumLookupResult = Result.Ok(expectedForumEditDto);
        
        _mockForumService
            .Setup(s => s.GetForumForEditAsync(1))
            .ReturnsAsync(forumLookupResult);
        
        _pageModel.ModelState.AddModelError("Name", "Name is required");
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.EditForumDto.Should().BeEquivalentTo(expectedForumEditDto);
        _mockForumService.Verify(s => s.GetForumForEditAsync(1), Times.Once);
        _mockForumService.Verify(s => s.UpdateForumAsync(1, It.IsAny<EditForumDto>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WhenForumUpdateFails_AddsModelErrorAndReturnPage()
    {
        var editForumDto = new EditForumDto()
        {
            Id = 1,
            Name = "test",
            Description = "test"
        };
        
        var forumLookupResult = Result.Ok(editForumDto);
        var forumUpdateResult = Result.Fail("Name is already in use");

        _mockForumService
            .Setup(s => s.GetForumForEditAsync(1))
            .ReturnsAsync(forumLookupResult);
        
        _mockForumService
            .Setup(s => s.UpdateForumAsync(1, editForumDto))
            .ReturnsAsync(forumUpdateResult);
        
        var result = await _pageModel.OnPostAsync(1);
        result.Should().BeOfType<PageResult>();
        _pageModel.EditForumDto.Should().BeEquivalentTo(editForumDto);
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _mockForumService.Verify(s => s.GetForumForEditAsync(1), Times.Once);
        _mockForumService.Verify(s => s.UpdateForumAsync(1, editForumDto), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenForumUpdateSucceeds_ReturnsRedirectToPageResult()
    {
        var editForumDto = new EditForumDto()
        {
            Id = 1,
            Name = "test",
            Description = "test"
        };

        var forumUpdateResult = Result.Ok();
        var forumLookupResult = Result.Ok(editForumDto);
        
        _mockForumService
            .Setup(s => s.GetForumForEditAsync(1))
            .ReturnsAsync(forumLookupResult);
        
        _mockForumService
            .Setup(s => s.UpdateForumAsync(1, editForumDto))
            .ReturnsAsync(forumUpdateResult);
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<RedirectToPageResult>();
        _mockForumService.Verify(s => s.GetForumForEditAsync(1), Times.Once);
        _mockForumService.Verify(s => s.UpdateForumAsync(1, editForumDto), Times.Once);
        _pageModel.EditForumDto.Should().BeEquivalentTo(editForumDto);
        _pageModel.ModelState.ErrorCount.Should().Be(0);
    }
}