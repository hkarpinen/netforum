using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Pages.Category;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class CategoryEditModelTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly EditModel _pageModel;

    public CategoryEditModelTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _pageModel = new EditModel(_mockCategoryService.Object);
    }

    [Fact]
    public async Task OnGetAsync_WhenCategoryIsFound_ShouldPopulateForm()
    {
        var editCategoryDto = new EditCategoryDto()
        {
            Name = "Test",
            Description = "Test"
        };
        
        var editCategoryDtoResult = Result.Ok(editCategoryDto);

        _mockCategoryService
            .Setup(s => s.GetCategoryForEditAsync(1))
            .ReturnsAsync(editCategoryDtoResult);
        
        var result = await _pageModel.OnGetAsync(1);

        result.Should().BeOfType<PageResult>();
        _pageModel.EditCategoryDto.Should().BeEquivalentTo(editCategoryDto);
    }

    [Fact]
    public async Task OnPostAsync_WhenCategoryIsNotFound_ReturnsNotFound()
    {
        var categoryLookupResult = Result.Fail<EditCategoryDto>("Category not found");
        
        _mockCategoryService
            .Setup(s => s.GetCategoryForEditAsync(1))
            .ReturnsAsync(categoryLookupResult);

        var result = await _pageModel.OnGetAsync(1);
        
        result.Should().BeOfType<NotFoundResult>();
    }
}