using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Posts;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Services;

namespace NETForum.UnitTests;

public class PostDetailModelTests
{
    private readonly Mock<IForumService> _mockForumService;
    private readonly Mock<IPostService> _mockPostService;
    private readonly Mock<IReplyService> _mockReplyService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly DetailModel _pageModel;

    public PostDetailModelTests()
    {
        _mockForumService = new Mock<IForumService>();
        _mockPostService = new Mock<IPostService>();
        _mockReplyService = new Mock<IReplyService>();
        _mockUserService = new Mock<IUserService>();
        _pageModel = new DetailModel(
            _mockForumService.Object,
            _mockPostService.Object,
            _mockReplyService.Object,
            _mockUserService.Object
        );
    }
    
    private void SetupAuthenticatedUser(PageModel pageModel, string username)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _pageModel.PageContext = new PageContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            }
        };
    }

    private void SetupSuccessfulPostLoad(int postId = 1, int authorId = 1)
    {
        // Setup authenticated user
        SetupAuthenticatedUser(_pageModel, "test");
        
        var user = new User { Id = authorId, UserName = "test", CreatedAt = DateTime.UtcNow.AddYears(-1) };
        _mockUserService
            .Setup(s => s.GetByUsernameAsync("test"))
            .ReturnsAsync(Result<User>.Success(user));

        // Setup post
        var post = new Post 
        { 
            Id = postId, 
            Title = "Test Post Title",
            Content = "Test Post Content",
            AuthorId = authorId,
            Author = user,
            Replies = new List<Reply>()
        };
        _mockPostService
            .Setup(s => s.GetPostWithAuthorAndRepliesAsync(postId))
            .ReturnsAsync(Result<Post>.Success(post));

        // Setup counts
        _mockPostService
            .Setup(s => s.GetTotalPostCountByAuthorAsync(authorId))
            .ReturnsAsync(10);
        
        _mockReplyService
            .Setup(s => s.GetTotalReplyCountAsync(authorId))
            .ReturnsAsync(25);

        // Setup breadcrumbs
        _mockForumService
            .Setup(s => s.GetForumBreadcrumbItems(It.IsAny<int>()))
            .ReturnsAsync(new List<BreadcrumbItemModel>());
    }

    [Fact]
    public async Task OnGetAsync_WhenPostIsFound_ShouldPopulateDetailsAndReturnPageResult()
    {
        SetupAuthenticatedUser(_pageModel, "test");
        SetupSuccessfulPostLoad(1, 2);
        
        var result = await _pageModel.OnGetAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.AuthenticatedUser.Should().NotBeNull();
        _pageModel.Post.Should().NotBeNull();
        _pageModel.Replies.Should().NotBeNull();
        _pageModel.UserIsAuthor.Should().BeTrue();
        _pageModel.AuthorTotalPosts.Should().Be(10);
        _pageModel.AuthorTotalReplies.Should().Be(25);
    }
    
    [Fact]
    public async Task OnPostAddReplyAsync_WhenModelStateIsInvalid_ReturnsPageResult()
    {
        SetupAuthenticatedUser(_pageModel, "test");
        SetupSuccessfulPostLoad(1, 2);
        
        _pageModel.ModelState.AddModelError("Reply.NoContent", "Reply must have content");

        var result = await _pageModel.OnPostAddReplyAsync(1);
        
        result.Should().BeOfType<PageResult>();
    }
    
    [Fact]
    public async Task OnPostAddReplyAsync_WhenReplyAddSucceeds_ReturnsRedirectToPageResult()
    {
        SetupAuthenticatedUser(_pageModel, "test");
        SetupSuccessfulPostLoad(1, 2);

        var replyAuthor = new User()
        {
            Id = 2,
            UserName = "test5",
            Email = "test5@test.com"
        };

        var expectedReply = new Reply()
        {
            Author = replyAuthor,
            Content = "Test Reply",
        };

        var createReplyDto = new CreatePostReplyDto()
        {
            Content = "Test Reply"
        };
        
        _pageModel.CreatePostReplyDto = createReplyDto;
        
        var addReplyResult = Result<Reply>.Success(expectedReply);
        
        _mockReplyService
            .Setup(s => s.AddReplyAsync(1, 2, createReplyDto))
            .ReturnsAsync(addReplyResult);
        
        var result = await _pageModel.OnPostAddReplyAsync(1);
        
        result.Should().BeOfType<RedirectToPageResult>();
        _pageModel.ModelState.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task OnPostAddReplyAsync_WhenReplyAddFails_ShouldAddModelErrorAndReturnPageResult()
    {
        SetupAuthenticatedUser(_pageModel, "test");
        SetupSuccessfulPostLoad(1, 2);

        var createReplyDto = new CreatePostReplyDto()
        {
            Content = "Test Reply"
        };
        
        _pageModel.CreatePostReplyDto = createReplyDto;
        
        var addReplyResult = Result<Reply>.Failure(new Error("Reply.NoContent", "Reply must have content"));
        
        _mockReplyService
            .Setup(s => s.AddReplyAsync(1, 2, createReplyDto))
            .ReturnsAsync(addReplyResult);
        
        var result = await _pageModel.OnPostAddReplyAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.IsValid.Should().BeFalse();
    }
    
}