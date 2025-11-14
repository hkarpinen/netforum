using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.Entities;
using NETForum.Pages.Admin;
using NETForum.Repositories.Filters;
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

        var expectedResult = new PagedResult<Category>()
        {
            Items = new List<Category>(),
            PageNumber = _pageModel.PageNumber,
            PageSize = _pageModel.PageSize,
            TotalCount = 0
        };
        
        _mockCategoryService
            .Setup(s => s.GetCategoriesWithForumsPagedAsync(
                1, 
                10,
                It.Is<CategoryFilterOptions>(f =>
                    f.Name == "Test" &&
                    f.Published == false
                )
            )).ReturnsAsync(expectedResult);
        
        var result = await _pageModel.OnGetAsync();
        
        _mockCategoryService.Verify(c => c.GetCategoriesWithForumsPagedAsync(
            1,
            10,
            It.Is<CategoryFilterOptions>(f =>
                f.Name == "Test" &&
                f.Published == false
            )
        ), Times.Once);

        result.Should().BeOfType<PageResult>();
    }
    
    
}