using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Filters;
using NETForum.Models;
using NETForum.Models.Entities;
using NETForum.Pages.Admin.Categories;
using NETForum.Services;

namespace NETForum.UnitTests;

public class AdminCategoriesManagementTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly CategoriesModel _pageModel;

    public AdminCategoriesManagementTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _pageModel = new CategoriesModel(_mockCategoryService.Object);
    }

    [Fact]
    public async Task OnGetAsync_CallsServiceWithCorrectParameters()
    {
        _pageModel.PageNumber = 1;
        _pageModel.PageSize = 10;
        _pageModel.Name = "Test";
        _pageModel.Published = false;

        var expectedResult = new PagedList<Category>(
            new List<Category>(),
            0,
            _pageModel.PageNumber,
            _pageModel.PageSize
        );

        var categoryFilterOptions = new CategoryFilterOptions
        {
            PageNumber = 1,
            PageSize = 10,
            Name = "Test",
            Published = false
        };
        
        _mockCategoryService
            .Setup(s => s.GetCategoriesPagedAsync(categoryFilterOptions))
            .ReturnsAsync(expectedResult);
        
        var result = await _pageModel.OnGetAsync();
        
        _mockCategoryService.Verify(s => s.GetCategoriesPagedAsync(It.IsAny<CategoryFilterOptions>()), Times.Once);

        result.Should().BeOfType<PageResult>();
    }
}