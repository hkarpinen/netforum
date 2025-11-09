using AutoMapper;
using FluentAssertions;
using Moq;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;
using NETForum.Services;

namespace NETForum.UnitTests;

public class PostServiceTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly PostService _systemUnderTest;

    public PostServiceTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockUserService = new Mock<IUserService>();
        _mockPostRepository = new Mock<IPostRepository>();
        _systemUnderTest = new PostService(_mockMapper.Object, _mockUserService.Object,  _mockPostRepository.Object);
    }
    
    [Fact]
    public async Task GetPostsAsync_WithValidForumId_ReturnsPosts()
    {
        var forumId = 1;
        var expectedPosts = new List<Post>()
        {
            new() { Id = 1, ForumId = forumId, Title = "Test Post 1", Content="Content 1" },
            new() { Id = 2, ForumId = forumId, Title = "Test Post 2", Content="Content 2" }
        };
        var navigations = new[] { "Author", "Replies.Author" };
        
        _mockPostRepository
            .Setup(r => r.GetPostsInForumAsync(forumId, navigations))
            .ReturnsAsync(expectedPosts);

        var result = await _systemUnderTest.GetPostsAsync(forumId);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedPosts);
        _mockPostRepository.Verify(r => r.GetPostsInForumAsync(forumId, navigations), Times.Once);
    }
    
    [Fact]
    public async Task GetPostsAsync_WithEmptyForum_ReturnsEmptyCollection()
    {
        var forumId = 1;
        var expectedPosts = new List<Post>();
        var navigations = new[] { "Author", "Replies.Author" };
        _mockPostRepository
            .Setup(r => r.GetPostsInForumAsync(forumId, navigations))
            .ReturnsAsync(expectedPosts);
        var result = await _systemUnderTest.GetPostsAsync(forumId);
        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task CreatePostAsync_WithValidData_ReturnsNewPost()
    {
        var forumId = 1;
        var username = "testuser";
        var createPostDto = new CreatePostDto
        {
            Title = "New Post",
            Content = "New Content"
        };
        var user = new User { Id = 1, UserName = username };
        var mappedPost = new Post
        {
            Title = createPostDto.Title,
            Content = createPostDto.Content,
        };
        var createdPost = new Post
        {
            Id = 1,
            Title = createPostDto.Title,
            Content = createPostDto.Content,
            ForumId = forumId,
            AuthorId = user.Id
        };
        
        _mockUserService
            .Setup(s => s.GetUserAsync(username))
            .ReturnsAsync(user);
        _mockMapper
            .Setup(m => m.Map<Post>(createPostDto))
            .Returns(mappedPost);
        _mockPostRepository
            .Setup(r => r.AddAsync(mappedPost))
            .ReturnsAsync(createdPost);
        
        var result = await _systemUnderTest.CreatePostAsync(username, forumId, createPostDto);
        
        result.Should().NotBeNull();
        result.Id.Should().Be(createdPost.Id);
        result.Title.Should().Be(createdPost.Title);
        result.Content.Should().Be(createdPost.Content);
        result.ForumId.Should().Be(forumId);
        result.AuthorId.Should().Be(user.Id);
        
        _mockUserService.Verify(s => s.GetUserAsync(username), Times.Once);
        _mockMapper.Verify(m => m.Map<Post>(createPostDto), Times.Once);
        _mockPostRepository.Verify(r => r.AddAsync(mappedPost), Times.Once);
    }
    
    [Fact]
    public async Task CreatePostAsync_WithNoAuthor_ThrowsException()
    {
        var forumId = 1;
        var username = "testuser";
        var createPostDto = new CreatePostDto
        {
            Title = "New Post",
            Content = "New Content"
        };
        
        _mockUserService
            .Setup(s => s.GetUserAsync(username))
            .ReturnsAsync((User)null);
        
        await Assert.ThrowsAsync<NullReferenceException>(async () => 
            await _systemUnderTest.CreatePostAsync(username, forumId, createPostDto));
    }
    
    [Fact]
    public async Task GetPostAsync_WithValidId_ReturnsPost()
    {
        var postId = 1;
        var expectedPost = new Post
        {
            Id = postId, 
            Title = "Test Post 1", 
            Content = "Content 1",
            AuthorId = 1
        };
        var navigations = new[] { "Author", "Replies" };
        _mockPostRepository
            .Setup(r => r.GetByIdAsync(postId, navigations))
            .ReturnsAsync(expectedPost);
        
        var result = await _systemUnderTest.GetPostWithAuthorAndRepliesAsync(postId);
        
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedPost.Id);
        result.Title.Should().Be(expectedPost.Title);
        result.AuthorId.Should().Be(expectedPost.AuthorId);
        _mockPostRepository.Verify(r => r.GetByIdAsync(postId, navigations), Times.Once);
    }

    [Fact]
    public async Task GetPostAsync_WithInvalidId_ReturnsNull()
    {
        var invalidPostId = 1000;
        var navigations = new[] { "Author", "Replies" };
        _mockPostRepository
            .Setup(r => r.GetByIdAsync(invalidPostId, navigations))
            .ReturnsAsync((Post?)null);
        var result = await _systemUnderTest.GetPostWithAuthorAndRepliesAsync(invalidPostId);
        result.Should().BeNull();
    }
    
    [Fact]
    public async Task GetTotalPostCountAsync_WhenNoPosts_ShouldReturnZero()
    {
        _mockPostRepository
            .Setup(r => r.GetTotalPostCountAllTime())
            .ReturnsAsync(0);
        var result = await _systemUnderTest.GetTotalPostCountAsync();
        result.Should().Be(0);
    }
}