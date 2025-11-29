using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Posts;
using NETForum.Services;
using FluentResults;

namespace NETForum.UnitTests;

public class PostDetailModelTests
{
    private readonly Mock<IPostService> _mockPostService;
    private readonly Mock<IReplyService> _mockReplyService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly DetailModel _pageModel;

    public PostDetailModelTests()
    {
        _mockPostService = new Mock<IPostService>();
        _mockReplyService = new Mock<IReplyService>();
        _mockUserService = new Mock<IUserService>();
        _pageModel = new DetailModel(_mockPostService.Object,
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

    private PostPageDto SetupSuccessfulPostLoad(int postId = 1, int authorId = 1)
    {
        // Setup authenticated user
        SetupAuthenticatedUser(_pageModel, "test");
        
        var user = new User { Id = authorId, UserName = "test", CreatedAt = DateTime.UtcNow.AddYears(-1) };
        _mockUserService
            .Setup(s => s.GetUserAsync("test"))
            .ReturnsAsync(Result.Ok(user));

        // Setup post
        var postPageDto = new PostPageDto
        {
            Id = 1,
            AuthorName = "test",
            AuthorId = 2,
            Title = "Test Post Title",
            Content = "Test Post Content",
            Replies = [],
            AuthorStatsSummary = new AuthorStatsSummary()
            {
                JoinedOn = DateTime.UtcNow,
                TotalPostCount = 1,
                TotalReplyCount = 1
            }
        };
        var postPageDtoResult = Result.Ok(postPageDto);
        
        _mockPostService
            .Setup(s => s.GetPostPageDto(1, "test"))
            .ReturnsAsync(postPageDtoResult);
        
        return postPageDto;
    }

    [Fact]
    public async Task OnGetAsync_WhenPostIsFound_ShouldPopulateDetailsAndReturnPageResult()
    {
        SetupAuthenticatedUser(_pageModel, "test");
        
        var postPageDto = SetupSuccessfulPostLoad(1, 2);
        
        var result = await _pageModel.OnGetAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.PostPageDto.Should().BeEquivalentTo(postPageDto);
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

        var addReplyResult = Result.Ok(expectedReply);
        
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
        
        var addReplyResult = Result.Fail<Reply>("Could not add reply.");
        
        _mockReplyService
            .Setup(s => s.AddReplyAsync(1, 2, createReplyDto))
            .ReturnsAsync(addReplyResult);
        
        var result = await _pageModel.OnPostAddReplyAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.IsValid.Should().BeFalse();
    }
}