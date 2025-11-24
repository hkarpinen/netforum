using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Forums;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class ForumCreateModelTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly Mock<IForumService> _mockForumService;
    private readonly CreateModel _pageModel;

    public ForumCreateModelTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _mockForumService = new Mock<IForumService>();
        _pageModel = new CreateModel(_mockForumService.Object, _mockCategoryService.Object);
    }

    [Fact]
    public async Task OnGetAsync_ShouldPopulateSelectListItems()
    {
        var parentForumSelectListItems = new List<SelectListItem>()
        {
            new() { Value = "1", Text = "1" },
            new() { Value = "2", Text = "2" },
        };

        var categorySelectListItems = new List<SelectListItem>()
        {
            new() { Value = "3", Text = "3" },
            new() { Value = "4", Text = "4" },
        };
        
        _mockCategoryService
            .Setup(s => s.GetCategorySelectListItemsAsync())
            .ReturnsAsync(categorySelectListItems);
        
        _mockForumService
            .Setup(s => s.GetForumSelectListItemsAsync())
            .ReturnsAsync(parentForumSelectListItems);
        
        var result = await _pageModel.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _pageModel.CategorySelectListItems.Should().BeEquivalentTo(categorySelectListItems);
        _pageModel.ParentForumSelectListItems.Should().BeEquivalentTo(parentForumSelectListItems);
        _mockCategoryService.Verify(s => s.GetCategorySelectListItemsAsync(), Times.Once);
        _mockForumService.Verify(s => s.GetForumSelectListItemsAsync(), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_ShouldPopulateSelectListItems()
    {
        var parentForumSelectListItems = new List<SelectListItem>()
        {
            new() { Value = "1", Text = "1" },
            new() { Value = "2", Text = "2" },
        };

        var categorySelectListItems = new List<SelectListItem>()
        {
            new() { Value = "3", Text = "3" },
            new() { Value = "4", Text = "4" },
        };
        
        _mockCategoryService
            .Setup(s => s.GetCategorySelectListItemsAsync())
            .ReturnsAsync(categorySelectListItems);
        
        _mockForumService
            .Setup(s => s.GetForumSelectListItemsAsync())
            .ReturnsAsync(parentForumSelectListItems);
        
        var result = await _pageModel.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        _pageModel.CategorySelectListItems.Should().BeEquivalentTo(categorySelectListItems);
        _pageModel.ParentForumSelectListItems.Should().BeEquivalentTo(parentForumSelectListItems);
        _mockCategoryService.Verify(s => s.GetCategorySelectListItemsAsync(), Times.Once);
        _mockForumService.Verify(s => s.GetForumSelectListItemsAsync(), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateInvalid_ReturnsPage()
    {
        _pageModel.ModelState.AddModelError("error", "error");
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        _mockCategoryService.Verify(s => s.GetCategorySelectListItemsAsync(), Times.Once);
        _mockForumService.Verify(s => s.GetForumSelectListItemsAsync(), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WhenForumAddFails_AddsModelErrorAndReturnsPage()
    {
        var parentForumSelectListItems = new List<SelectListItem>()
        {
            new() { Value = "1", Text = "1" },
            new() { Value = "2", Text = "2" },
        };

        var categorySelectListItems = new List<SelectListItem>()
        {
            new() { Value = "3", Text = "3" },
            new() { Value = "4", Text = "4" },
        };

        var createForumDto = new CreateForumDto()
        {
            Name = "name",
            Description = "description",
            CategoryId = 1,
        };
        
        var forumAddResult = Result.Fail("Error");
        
        _pageModel.CreateForumDto = createForumDto;
        
        _mockCategoryService
            .Setup(s => s.GetCategorySelectListItemsAsync())
            .ReturnsAsync(categorySelectListItems);
        
        _mockForumService
            .Setup(s => s.GetForumSelectListItemsAsync())
            .ReturnsAsync(parentForumSelectListItems);
        
        _mockForumService
            .Setup(s => s.AddForumAsync(createForumDto))
            .ReturnsAsync(forumAddResult);

        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _mockCategoryService.Verify(s => s.GetCategorySelectListItemsAsync(), Times.Once);
        _mockForumService.Verify(s => s.GetForumSelectListItemsAsync(), Times.Once);
        _mockForumService.Verify(s => s.AddForumAsync(createForumDto), Times.Once);
        _pageModel.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async Task OnPostAsync_WhenForumAddIsSuccessful_ShouldRedirect()
    {
        var parentForumSelectListItems = new List<SelectListItem>()
        {
            new() { Value = "1", Text = "1" },
            new() { Value = "2", Text = "2" },
        };

        var categorySelectListItems = new List<SelectListItem>()
        {
            new() { Value = "3", Text = "3" },
            new() { Value = "4", Text = "4" },
        };

        var createForumDto = new CreateForumDto()
        {
            Name = "name",
            Description = "description",
            CategoryId = 1,
        };

        var createdForum = new Forum()
        {
            Id = 1,
            Name = "name",
            Description = "description",
            CategoryId = 1
        };

        var forumAddResult = Result.Ok(createdForum);
        
        _pageModel.CreateForumDto = createForumDto;
        
        _mockCategoryService
            .Setup(s => s.GetCategorySelectListItemsAsync())
            .ReturnsAsync(categorySelectListItems);
        
        _mockForumService
            .Setup(s  => s.GetForumSelectListItemsAsync())
            .ReturnsAsync(parentForumSelectListItems);
        
        _mockForumService
            .Setup(s => s.AddForumAsync(createForumDto))
            .ReturnsAsync(forumAddResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<RedirectToPageResult>();
        _mockCategoryService.Verify(s => s.GetCategorySelectListItemsAsync(), Times.Once);
        _mockForumService.Verify(s => s.GetForumSelectListItemsAsync(), Times.Once);
        _mockForumService.Verify(s => s.AddForumAsync(createForumDto), Times.Once);
        _pageModel.ModelState.ErrorCount.Should().Be(0);
    }
}