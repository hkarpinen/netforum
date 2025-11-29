using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services;
using FluentResults;
using NETForum.Pages.Admin.Categories;

namespace NETForum.UnitTests;

public class CategoryCreateModelTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly CreateModel _pageModel;

    public CategoryCreateModelTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _pageModel = new CreateModel(_mockCategoryService.Object);
    }

    [Fact]
    public async Task OnPostAsync_WhenModelStateIsInvalid_ReturnsPage()
    {
        _pageModel.ModelState.AddModelError("Name", "Name is required.");
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<PageResult>();
        _mockCategoryService.Verify(s => s.AddCategoryAsync(It.IsAny<CreateCategoryDto>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WhenCategoryIsSuccessfullyAdded_ShouldRedirect()
    {
        var createCategoryDto = new CreateCategoryDto()
        {
            Name = "Test",
            Description = "Test description",
            Published = false,
            SortOrder = 1
        };

        var expectedCategory = new Category()
        {
            Id = 1,
            Name = "Test",
            Description = "Test description",
            Published = false,
            SortOrder = 1
        };
        
        _pageModel.CreateCategoryDto = createCategoryDto;
        
        var categoryCreateResult = Result.Ok(expectedCategory);
        
        _mockCategoryService
            .Setup(s => s.AddCategoryAsync(createCategoryDto))
            .ReturnsAsync(categoryCreateResult);
        
        var result = await _pageModel.OnPostAsync();
        
        result.Should().BeOfType<RedirectToPageResult>();
        _mockCategoryService.Verify(s => s.AddCategoryAsync(It.IsAny<CreateCategoryDto>()), Times.Once);
    }
    
    [Fact]
    public async Task OnPostAsync_WhenCategoryFailsToCreate_ShouldAddModelErrorAndReturnPageResult()
    {
        var createCategoryDto = new CreateCategoryDto()
        {
            Name = "Test",
            Description = "Test description",
            Published = false,
            SortOrder = 1
        };
        
        _pageModel.CreateCategoryDto = createCategoryDto;

        var categoryCreateResult = Result.Fail<Category>("Name is already in use.");
        
        _mockCategoryService
            .Setup(s => s.AddCategoryAsync(createCategoryDto))
            .ReturnsAsync(categoryCreateResult);
        
        var result = await _pageModel.OnPostAsync();
        
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        result.Should().BeOfType<PageResult>();


    }
}