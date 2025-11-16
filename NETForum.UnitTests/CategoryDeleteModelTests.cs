using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NETForum.Pages.Category;
using NETForum.Services;

namespace NETForum.UnitTests;

public class CategoryDeleteModelTests
{
    private readonly DeleteModel _pageModel;

    public CategoryDeleteModelTests()
    {
        var mockCategoryService = new Mock<ICategoryService>();
        _pageModel = new DeleteModel(mockCategoryService.Object);
    }

    [Fact]
    public async Task OnGetAsync_ShouldDeleteCategoryAndRedirect()
    {
        var result = await _pageModel.OnGetAsync(1);
        result.Should().BeOfType<RedirectToPageResult>();
    }
}