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

public class PostCreateModelTests
{
    private readonly Mock<IPostService> _mockPostService;
    private readonly CreateModel _pageModel;

    public PostCreateModelTests()
    {
        _mockPostService = new Mock<IPostService>();
        _pageModel = new CreateModel(_mockPostService.Object);
    }

    private void SeedIdentity(PageModel pageModel, string username)
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

    [Fact]
    public async Task OnPostAsync_WhenModelStateIsInvalid_ReturnsPageResult()
    {
        _pageModel.ModelState.AddModelError("key", "error");
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.Count.Should().Be(1);
        _mockPostService.Verify(s => s.AddPostAsync(
            It.IsAny<string>(), 
            It.IsAny<int>(),
            It.IsAny<CreatePostDto>()
        ), Times.Never);
    }
    
    [Fact]
    public async Task OnPostAsync_WhenPostAddFails_AddsModelErrorAndReturnsPageResult()
    {
        SeedIdentity(_pageModel, "testuser");
        
        var createPostDto = new CreatePostDto()
        {
            Title = "title",
            Content = "content",
            IsLocked = false
        };
        
        _pageModel.CreatePostDto = createPostDto;

        var addPostResult = Result.Fail("Title exists");
        _mockPostService
            .Setup(s => s.AddPostAsync(It.IsAny<string>(), 1, createPostDto))
            .ReturnsAsync(addPostResult);
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task OnPostAsync_WhenPostAddSucceeds_ReturnsRedirectToPageResult()
    {
        SeedIdentity(_pageModel, "testuser");
        
        var createPostDto = new CreatePostDto()
        {
            Title = "title",
            Content = "content",
            IsLocked = false
        };

        var expectedPost = new Post()
        {
            Id = 1,
            Title = "title",
            Content = "content",
            ForumId = 1,
            IsLocked = false,
            AuthorId = 1
        };
        
        _pageModel.CreatePostDto = createPostDto;
        
        var postAddResult = Result.Ok(expectedPost);
        
        _mockPostService
            .Setup(s => s.AddPostAsync("testuser", 1, createPostDto))
            .ReturnsAsync(postAddResult);
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<RedirectToPageResult>();
        _pageModel.ModelState.Count.Should().Be(0);
    }
}