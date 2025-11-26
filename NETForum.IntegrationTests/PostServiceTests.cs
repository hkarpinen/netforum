using EntityFramework.Exceptions.Sqlite;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Errors;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.IntegrationTests;

public class PostServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IPostService _postService;
    private readonly SqliteConnection _connection;

    public PostServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .UseExceptionProcessor()
            .Options;
        
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        
        _postService = new PostService(_context);
        
        SeedDatabase();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _connection.Dispose();
    }

    private void SeedDatabase()
    {
        var author1 = new User()
        {
            Id = 1,
            ProfileImageUrl = null,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UserName = "test",
            Email = "test@email.com"
        };
        
        _context.Users.AddRange(author1);
        
        var forum = new Forum()
        {
            Id = 1,
            Name = "Test Forum",
            Description = "Test Forum Description",
            Published = true,
            ParentForumId = null,
            CategoryId = null,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        
        _context.Forums.AddRange(forum);

        var post1 = new Post()
        {
            Id = 1,
            ForumId = forum.Id,
            AuthorId = author1.Id,
            Title = "Test Post",
            Content = "Test Content",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Published = true,
            IsPinned = false,
            IsLocked = false,
            ViewCount = 1
        };

        var post2 = new Post()
        {
            Id = 2,
            ForumId = forum.Id,
            AuthorId = author1.Id,
            Title = "Test Post 2",
            Content = "Test Content 2",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Published = false,
            IsPinned = false,
            IsLocked = false,
            ViewCount = 1
        };
        
        _context.Posts.AddRange(post1, post2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetPostForEditAsync_WithValidId_ReturnsExpectedResult()
    {
        var postEditDtoResult = await _postService.GetPostForEditAsync(1);
        postEditDtoResult.IsSuccess.Should().BeTrue();
        var postEditDto = postEditDtoResult.Value;
        postEditDto.Title.Should().Be("Test Post");
        postEditDto.Content.Should().Be("Test Content");
        postEditDto.AuthorId.Should().Be(1);
        postEditDto.ForumId.Should().Be(1);
    }

    [Fact]
    public async Task GetPostForEditAsync_WithInvalidId_ReturnsFailureResult()
    {
        var postEditDtoResult = await _postService.GetPostForEditAsync(-1);
        postEditDtoResult.IsSuccess.Should().BeFalse();
        postEditDtoResult.Errors.Should().HaveCount(1);
        postEditDtoResult.Errors[0].Message.Should().Be(PostErrors.NotFound(-1).Message);
    }

    [Fact]
    public async Task UpdatePostAsync_WithInvalidId_ReturnsFailureResult()
    {
        var editPostDto = new EditPostDto()
        {
            Title = "Test Post",
            Content = "Test Content",
            ForumId = 1,
            AuthorId = 1,
            IsPinned = false,
            IsLocked = false,
            Published = false
        };
        
        var result = await _postService.UpdatePostAsync(-1, editPostDto);
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(PostErrors.NotFound(-1).Message);
    }

    [Fact]
    public async Task UpdatePostAsync_WithValidIdAndDto_ShouldUpdatePost()
    {
        var editPostDto = new EditPostDto()
        {
            Title = "Test Post 2",
            Content = "Test Content 2",
            ForumId = 1,
            AuthorId = 1,
            IsPinned = false,
            IsLocked = false,
            Published = false
        };
        
        var result = await _postService.UpdatePostAsync(1, editPostDto);
        
        result.IsSuccess.Should().BeTrue();

        var updatedPost = await _postService.GetPostAsync(1);
        updatedPost.Value.Title.Should().Be("Test Post 2");
        updatedPost.Value.Content.Should().Be("Test Content 2");
    }

    [Fact]
    public async Task UpdatePostAsync_WithNonExistentForumId_ReturnsFailureResult()
    {
        var editPostDto = new EditPostDto()
        {
            Title = "Test Post",
            Content = "Test Content",
            ForumId = -1,
            AuthorId = 1,
            IsPinned = false,
            IsLocked = false,
            Published = false
        };
        
        var result = await _postService.UpdatePostAsync(1, editPostDto);
        
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(PostErrors.NonExistentForumId(-1).Message);
    }

    [Fact]
    public async Task UpdatePostAsync_WithInvalidAuthorId_ReturnsFailureResult()
    {
        var editPostDto = new EditPostDto()
        {
            Title = "Test Post",
            Content = "Test Content",
            ForumId = 1,
            AuthorId = -1,
            IsPinned = false,
            IsLocked = false,
            Published = false
        };
        
        var result = await _postService.UpdatePostAsync(1, editPostDto);
        
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(PostErrors.AuthorNotFound(editPostDto.AuthorId).Message);
    }

    [Fact]
    public async Task GetPostsPagedAsync_ReturnsExpectedResult()
    {
        var postFilterOptions = new PostFilterOptions()
        {
            Published = false
        };

        var result = await _postService.GetPostsPagedAsync(postFilterOptions);
        
        var items = result.Items.ToList();
        items.Count.Should().Be(1);
        items[0].Id.Should().Be(2);
        items[0].Title.Should().Be("Test Post 2");
        items[0].Content.Should().Be("Test Content 2");
        items[0].AuthorName.Should().Be("test");
        items[0].AuthorAvatarUrl.Should().BeNull();
    }

    [Fact]
    public async Task AddPostAsync_WithInvalidAuthorUsername_ReturnsFailureResult()
    {
        var createPostDto = new CreatePostDto()
        {
            Title = "Test Post",
            Content = "Test Content",
            IsPinned = false,
            IsLocked = false,
            Published = false
        };
        
        var result = await _postService.AddPostAsync("invalid_user", 1, createPostDto);
        
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(PostErrors.AuthorNotFound("invalid_user").Message);
    }

    [Fact]
    public async Task AddPostAsync_WithInvalidForumId_ReturnsFailureResult()
    {
        var createPostDto = new CreatePostDto()
        {
            Title = "Test Post",
            Content = "Test Content",
            IsPinned = false,
            IsLocked = false,
            Published = false
        };
        
        var result = await _postService.AddPostAsync("test", -1, createPostDto);
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(PostErrors.NonExistentForumId(-1).Message);
    }

    [Fact]
    public async Task AddPostAsync_WithValidDtoAndIds_ReturnsCreateEntity()
    {
        var createPostDto = new CreatePostDto()
        {
            Title = "Test Post 3",
            Content = "Test Content 3",
            IsPinned = true,
            IsLocked = false,
            Published = true
        };
        
        var result = await _postService.AddPostAsync("test", 1, createPostDto);
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(3);
        result.Value.Title.Should().Be("Test Post 3");
        result.Value.Content.Should().Be("Test Content 3");
        result.Value.AuthorId.Should().Be(1);
        result.Value.IsPinned.Should().BeTrue();
        result.Value.IsLocked.Should().BeFalse();
        result.Value.Published.Should().BeTrue();
    }
    
}