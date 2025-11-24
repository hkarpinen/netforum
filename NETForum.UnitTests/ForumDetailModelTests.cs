using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Pages.Forums;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class ForumDetailModelTests
{
    private readonly Mock<IForumService> _mockForumService;
    private readonly Mock<IPostService> _mockPostService;
    private readonly DetailsModel _pageModel;

    public ForumDetailModelTests()
    {
        _mockForumService = new Mock<IForumService>();
        _mockPostService = new Mock<IPostService>();
        _pageModel = new DetailsModel(_mockForumService.Object, _mockPostService.Object);
    }

    [Fact]
    public async Task OnGetAsync_WhenForumDoesNotExist_ReturnsNotFound()
    {
        var forumPageDtoResult = Result.Fail<ForumPageDto>("Forum not found");
        
        _mockForumService
            .Setup(s => s.GetForumPageDtoAsync(1))
            .ReturnsAsync(forumPageDtoResult);

        var result = await _pageModel.OnGetAsync(1);
        
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenForumExists_PopulatesDto()
    {
        var forumPageDto = new ForumPageDto
        {
            Id = 1,
            Title = "Test",
            Description = "Test description",
            Posts = [],
            Subforums = []
        };
        
        var forumPageDtoResult = Result.Ok(forumPageDto);
        
        _mockForumService
            .Setup(s => s.GetForumPageDtoAsync(1))
            .ReturnsAsync(forumPageDtoResult);
        
        var result = await _pageModel.OnGetAsync(1);
        
        _pageModel.ForumPageDto.Should().BeEquivalentTo(forumPageDto);
        result.Should().BeOfType<PageResult>();
    }
}