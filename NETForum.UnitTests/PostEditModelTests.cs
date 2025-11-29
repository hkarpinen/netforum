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

public class PostEditModelTests
{
    private readonly Mock<IPostService> _mockPostService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly EditModel _pageModel;

    public PostEditModelTests()
    {
        _mockPostService = new Mock<IPostService>();
        _mockUserService = new Mock<IUserService>();
        _pageModel = new EditModel(_mockPostService.Object, _mockUserService.Object);
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

    [Fact]
    public async Task OnGetAsync_WhenPostIsNotFound_ReturnsNotFoundResult()
    {
        SetupAuthenticatedUser(_pageModel, "TestUsername");
        var getEditPostDtoResult = Result.Fail<EditPostDto>("Post not found");
        
        _mockPostService
            .Setup(s => s.GetPostForEditAsync(1))
            .ReturnsAsync(getEditPostDtoResult);
        
        var result = await _pageModel.OnGetAsync(1);
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task OnGetAsync_WhenPostIsFoundWithAuthor_ShouldPopulateFormAndReturnPage()
    {
        SetupAuthenticatedUser(_pageModel, "TestUsername");
        var editPostDto = new EditPostDto()
        {
            AuthorId = 1,
            Content = "TestContent",
            Title = "TestTitle",
        };     
        var expectedUser = new User()
        {
            Id = 1,
            UserName = "TestUsername",
            Email = "TestEmail"
        };

        var getEditPostDtoResult = Result.Ok(editPostDto);
        var authorLookupResult = Result.Ok(expectedUser);
        
        _mockPostService
            .Setup(s => s.GetPostForEditAsync(1))
            .ReturnsAsync(getEditPostDtoResult);

        _mockUserService
            .Setup(s => s.GetUserAsync(1))
            .ReturnsAsync(authorLookupResult);
        
        var result = await _pageModel.OnGetAsync(1);
        _pageModel.EditPostDto.Should().BeEquivalentTo(editPostDto);
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task OnPostAsyncWhenPostIsNotFound_ReturnsNotFoundResult()
    {
        SetupAuthenticatedUser(_pageModel, "TestUsername");
        var getEditPostDtoResult = Result.Fail<EditPostDto>("Post not found");
        
        _mockPostService
            .Setup(s => s.GetPostForEditAsync(1))
            .ReturnsAsync(getEditPostDtoResult);
        
        var result = await _pageModel.OnPostAsync(1);
        
        result.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async Task OnPostAsync_WhenPostUpdateFails_AddsModelErrorAndReturnsPage()
    {
        SetupAuthenticatedUser(_pageModel, "TestUsername");
        
        var editPostDto = new EditPostDto()
        {
            AuthorId = 1,
            Content = "TestContent",
            Title = "TestTitle",
        };
        
        _pageModel.EditPostDto = editPostDto;
        
        var author = new User()
        {
            Id = 1,
            UserName = "TestUsername",
            Email = "TestEmail"
        };
        var getEditPostDtoResult = Result.Ok(editPostDto);
        var authorLookupResult = Result.Ok(author);
        var postUpdateResult = Result.Fail("Could not update post");
        
        _mockPostService
            .Setup(s => s.GetPostForEditAsync(1))
            .ReturnsAsync(getEditPostDtoResult);
        
        _mockUserService
            .Setup(s => s.GetUserAsync(1))
            .ReturnsAsync(authorLookupResult);
        
        _mockPostService
            .Setup(s => s.UpdatePostAsync(1, editPostDto))
            .ReturnsAsync(postUpdateResult);
        
        var result = await _pageModel.OnPostAsync(1);
        
        _pageModel.EditPostDto.Should().BeEquivalentTo(editPostDto);
        result.Should().BeOfType<PageResult>();
        _pageModel.ModelState.ErrorCount.Should().Be(1);
        
    }
}