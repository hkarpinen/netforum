using AutoMapper;
using EntityFramework.Exceptions.Common;
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
    public async Task AddPostAsync_WithValidData_ReturnsNewPost()
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
        
        var userLookupResult = Result<User>.Success(user);
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(username))
            .ReturnsAsync(userLookupResult);
        _mockMapper
            .Setup(m => m.Map<Post>(createPostDto))
            .Returns(mappedPost);
        _mockPostRepository
            .Setup(r => r.AddAsync(mappedPost))
            .ReturnsAsync(createdPost);
        
        var addPostResult = await _systemUnderTest.AddPostAsync(username, forumId, createPostDto);
        var result = addPostResult.Value;
        
        result.Should().NotBeNull();
        result.Id.Should().Be(createdPost.Id);
        result.Title.Should().Be(createdPost.Title);
        result.Content.Should().Be(createdPost.Content);
        result.ForumId.Should().Be(forumId);
        result.AuthorId.Should().Be(user.Id);
        
        _mockUserService.Verify(s => s.GetByUsernameAsync(username), Times.Once);
        _mockMapper.Verify(m => m.Map<Post>(createPostDto), Times.Once);
        _mockPostRepository.Verify(r => r.AddAsync(mappedPost), Times.Once);
    }

    [Fact]
    public async Task AddPostAsync_WithNonExistingAuthor_ReturnsFailureResult()
    {
        var forumId = 1;
        var username = "testuser";
        var createPostDto = new CreatePostDto
        {
            Title = "New Post",
            Content = "New Content"
        };
        
        var userLookupResult = Result<User>.Failure(new Error("User.NotFound", "Not Found"));
        
        _mockUserService
            .Setup(s => s.GetByUsernameAsync(username))
            .ReturnsAsync(userLookupResult);
        
        var result = await _systemUnderTest.AddPostAsync(username, forumId, createPostDto);
        
        result.IsFailure.Should().BeTrue();
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
        
        var postLookupResult = await _systemUnderTest.GetPostWithAuthorAndRepliesAsync(postId);
        var result = postLookupResult.Value;
        
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedPost.Id);
        result.Title.Should().Be(expectedPost.Title);
        result.AuthorId.Should().Be(expectedPost.AuthorId);
        _mockPostRepository.Verify(r => r.GetByIdAsync(postId, navigations), Times.Once);
    }

    [Fact]
    public async Task GetPostAsync_WithInvalidId_ReturnsFailedResult()
    {
        var invalidPostId = 1000;
        var navigations = new[] { "Author", "Replies" };
        _mockPostRepository
            .Setup(r => r.GetByIdAsync(invalidPostId, navigations))
            .ReturnsAsync((Post?)null);
        var result = await _systemUnderTest.GetPostWithAuthorAndRepliesAsync(invalidPostId);
        result.IsFailure.Should().BeTrue();
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

    [Fact]
    public async Task GetLatestPostsWithAuthorAsync_WithLimit_ReturnsLatestPostsWithAuthor()
    {
        var author = new User { Id = 1, UserName = "testuser" };
        
        var posts = new List<Post>()
        {
            new() { Title = "Test Post 1", Content = "Content 1", Author = author },
            new() { Title = "Test Post 2", Content = "Content 2", Author = author },
            new() { Title = "Test Post 3", Content =  "Content 3", Author = author }
        };
        var navigations = new[] { "Author" };
        var limit = 3;
        
        _mockPostRepository
            .Setup(r => r.GetLatestPostsAsync(limit, navigations))
            .ReturnsAsync(posts);

        var result =await _systemUnderTest.GetLatestPostsWithAuthorAsync(limit);
        
        result.Should().NotBeNull();
        result.Should().HaveCount(limit);
        result.Should().BeEquivalentTo(posts);
    }

    [Fact]
    public async Task AddPostAsync_WhenUniqueConstraintViolation_ReturnsFailureResult()
    {
        var username = "testuser";
        var user = new User { Id = 1, UserName = "testuser" };
        var forumId = 1;

        var createPostDto = new CreatePostDto()
        {
            Title = "Test Post 1",
            Content = "Content 1",
        };

        var mappedPost = new Post()
        {
            Id = 1,
            Title = "Test Post 1",
            Content = "Content 1",
        };

        _mockUserService
            .Setup(s => s.GetByUsernameAsync(username))
            .ReturnsAsync(Result<User>.Success(user));
        
        _mockMapper
            .Setup(s => s.Map<Post>(createPostDto))
            .Returns(mappedPost);
            
        var exception = new UniqueConstraintException();
        var constraintNameProperty = typeof(UniqueConstraintException)
            .GetProperty("ConstraintName");
        constraintNameProperty?.SetValue(exception, "IX_Posts_Title");
        
        _mockPostRepository
            .Setup(r => r.AddAsync(It.IsAny<Post>()))
            .ThrowsAsync(exception);
        
        var result = await _systemUnderTest.AddPostAsync(username, forumId, createPostDto);
        
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Title is already taken.");
    }
    
    [Fact]
    public async Task GetTotalPostCountByAuthorAsync_WithValidAuthorId_ReturnsTotalPostCount()
    {
        var totalPostCount = 50;
        var authorId = 1;
        
        _mockPostRepository
            .Setup(r => r.GetTotalPostCountByAuthorAsync(authorId))
            .ReturnsAsync(totalPostCount);
        
        var result = await _systemUnderTest.GetTotalPostCountByAuthorAsync(authorId);
        result.Should().Be(totalPostCount);
    }
}