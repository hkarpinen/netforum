using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.Entities;
using NETForum.Pages.Forums;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Pages.Shared.Components.ForumList;
using NETForum.Services;

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
        var forumLookupResult = Result<Forum>.Failure(new Error("Forum.NotFound", "Forum not found"));
        
        _mockForumService
            .Setup(s => s.GetForumByIdAsync(1))
            .ReturnsAsync(forumLookupResult);

        var result = await _pageModel.OnGetAsync(1);
        
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenForumExists_PopulatesDetails()
    {
        var forum = new Forum()
        {
            Id = 1,
            Name = "test",
            Description = "test"
        };

        var subForum = new Forum()
        {
            Id = 2,
            Name = "test2",
            Description = "test2"
        };

        var childForumListItems = new List<ForumListItemModel>()
        {
            new()
            {
                Forum = subForum,
                PostCount = 1,
                RepliesCount = 2
            }
        };

        var breadcrumbItems = new List<BreadcrumbItemModel>()
        {
            new()
            {
                Active = true,
                Text = "test",
                Url = "test"
            }
        };

        var forumPosts = new List<Post>()
        {
            new()
            {
                Id = 1,
                AuthorId = 1,
                Title = "test",
                Content = "test"
            }
        };
        
        var forumLookupResult = Result<Forum>.Success(forum);
        
        _mockForumService
            .Setup(s => s.GetForumByIdAsync(1))
            .ReturnsAsync(forumLookupResult);
        
        _mockForumService
            .Setup(s => s.GetForumBreadcrumbItems(1))
            .ReturnsAsync(breadcrumbItems);
        
        _mockForumService
            .Setup(s => s.GetChildForumListItemsWithPostsAndRepliesAsync(1))
            .ReturnsAsync(childForumListItems);

        _mockPostService
            .Setup(s => s.GetPostsAsync(1))
            .ReturnsAsync(forumPosts);
        
        var result = await _pageModel.OnGetAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.BreadcrumbItems.Should().BeEquivalentTo(breadcrumbItems);
        _pageModel.Posts.Should().BeEquivalentTo(forumPosts);
        _pageModel.Forum.Should().BeEquivalentTo(forum);
        _pageModel.ChildForumsWithPostAndReplies.Should().BeEquivalentTo(childForumListItems);
    }
}