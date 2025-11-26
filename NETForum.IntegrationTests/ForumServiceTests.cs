using EntityFramework.Exceptions.Sqlite;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NETForum.Data;
using NETForum.Errors;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.IntegrationTests;

public class ForumServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IForumService _forumService;
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly SqliteConnection _connection;

    public ForumServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open(); 
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .UseExceptionProcessor() 
            .Options;
        
        _mockMemoryCache = new Mock<IMemoryCache>();
        
        _context = new AppDbContext(options);
        
        _context.Database.EnsureCreated();

        _forumService = new ForumService(
            _context,
            _mockMemoryCache.Object
        );
        
        SeedDatabase();
    }
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedDatabase()
    {
        // Add users 
        var user1 = new User()
        {
            Id = 1,
            UserName = "user1",
            Email = "user1@email.com",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            ProfileImageUrl = "test.com/image.png"
        };

        var user2 = new User()
        {
            Id = 2,
            UserName = "user2",
            Email = "user2@email.com",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            ProfileImageUrl = "test.com/image2.png"
        };
        
        _context.Users.AddRange(user1, user2);
        
        // Add forums
        var rootForum = new Forum()
        {
            Id = 1,
            Name = "Test Forum",
            Description = "Test Forum Description",
            Published = true,
            ParentForumId = null,
            CategoryId = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var subForum = new Forum()
        {
            Id = 2,
            Name = "Test Sub Forum",
            Description = "Test Sub Forum Description",
            Published = true,
            ParentForumId = 1,
            CategoryId = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Forums.AddRange(rootForum, subForum);

        // Add posts
        var rootForumPost1 = new Post()
        {
            Id = 1,
            ForumId = 1,
            AuthorId = 1,
            IsPinned = false,
            IsLocked = false,
            Published = true,
            Title = "Test Post 1",
            Content = "Test Post 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ViewCount = 10,
            ReplyCount = 1
        };

        var subForumPost1 = new Post()
        {
            Id = 2,
            ForumId = 2,
            AuthorId = 2,
            IsPinned = false,
            IsLocked = false,
            Published = true,
            Title = "Test Post 2",
            Content = "Test Post 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ViewCount = 20,
            ReplyCount = 1
        };
        
        _context.Posts.AddRange(rootForumPost1, subForumPost1);

        var rootForumPost1Reply = new Reply()
        {
            Id = 1,
            PostId = 1,
            AuthorId = 1,
            Content = "Test Post 1",
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
        };

        var subForumPost1Reply = new Reply()
        {
            Id = 2,
            PostId = 2,
            AuthorId = 2,
            Content = "Test Post 2",
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
        };
        
        _context.Replies.AddRange(rootForumPost1Reply, subForumPost1Reply);
        
        // Save changes to DB
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetForumPageDto_WithValidForumIdReturnsExpectedResult()
    {
        var result = await _forumService.GetForumPageDtoAsync(1);
        
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value;
        
        // Validate DTO values are correct
        dto.Id.Should().Be(1);
        dto.Title.Should().Be("Test Forum");
        dto.Description.Should().Be("Test Forum Description");
        
        dto.Posts.Count.Should().Be(1);
        
        dto.Posts[0].Id.Should().Be(1);
        dto.Posts[0].Title.Should().Be("Test Post 1");
        dto.Posts[0].Content.Should().Be("Test Post 1");
        dto.Posts[0].AuthorName.Should().Be("user1");
        dto.Posts[0].AuthorAvatarUrl.Should().Be("test.com/image.png");
        dto.Posts[0].ReplyCount.Should().Be(1);
        dto.Posts[0].ViewCount.Should().Be(10);
        dto.Posts[0].LastReplySummary.Id.Should().Be(1);
        dto.Posts[0].LastReplySummary.AuthorName.Should().Be("user1");
        dto.Posts[0].LastReplySummary.AuthorAvatarUrl.Should().Be("test.com/image.png");
        
        dto.Subforums.Count.Should().Be(1);
        dto.Subforums[0].Id.Should().Be(2);
        dto.Subforums[0].Title.Should().Be("Test Sub Forum");
        dto.Subforums[0].Description.Should().Be("Test Sub Forum Description");
        dto.Subforums[0].TotalPosts.Should().Be(1);
        dto.Subforums[0].TotalReplies.Should().Be(1);
        
        dto.Subforums[0].LastPostSummary.Id.Should().Be(2);
        dto.Subforums[0].LastPostSummary.AuthorName.Should().Be("user2");
        dto.Subforums[0].LastPostSummary.AuthorAvatarUrl.Should().Be("test.com/image2.png");
        dto.Subforums[0].LastPostSummary.Title.Should().Be("Test Post 2");
        dto.Subforums[0].LastPostSummary.Content.Should().Be("Test Post 2");
        dto.Subforums[0].LastPostSummary.ReplyCount.Should().Be(1);
        dto.Subforums[0].LastPostSummary.ViewCount.Should().Be(20);
        dto.Subforums[0].LastPostSummary.LastReplySummary.Id.Should().Be(2);
        dto.Subforums[0].LastPostSummary.LastReplySummary.AuthorName.Should().Be("user2");
        dto.Subforums[0].LastPostSummary.LastReplySummary.AuthorAvatarUrl.Should().Be("test.com/image2.png");
    }

    [Fact]
    public async Task GetForumPageDto_WithInvalidForumId_ReturnsFailedResult()
    {
        var result = await _forumService.GetForumPageDtoAsync(-1);
        
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetForumsPagedAsync_WithParentForumId_ReturnsExpectedResult()
    {
        var forumFilterOptions = new ForumFilterOptions()
        {
            ParentForumId = 1
        };
        
        var result = await _forumService.GetForumsPagedAsync(forumFilterOptions);
        result.Items.Count.Should().Be(1);
        
        var itemsToList = result.Items.ToList();
        itemsToList[0].Id.Should().Be(2);
        itemsToList[0].Name.Should().Be("Test Sub Forum");
        itemsToList[0].Description.Should().Be("Test Sub Forum Description");
        itemsToList[0].Posts.Count.Should().Be(1);
        itemsToList[0].ParentForum.Id.Should().Be(1);
        itemsToList[0].ParentForum.Name.Should().Be("Test Forum");
    }

    [Fact]
    public async Task AddForumAsync_WithValidDto_ShouldAddForum()
    {
        var createForumDto = new CreateForumDto()
        {
            Name = "Test Forum 3",
            Description = "Test Forum Description 3",
            Published = false,
            ParentForumId = 1,
            CategoryId = null
        };

        var result = await _forumService.AddForumAsync(createForumDto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(3);
        result.Value.Name.Should().Be("Test Forum 3");
    }

    [Fact]
    public async Task AddForumAsync_WithExistingName_ReturnsFailureResult()
    {
        var createForumDto = new CreateForumDto()
        {
            Name = "Test Forum",
            Description = "Test Forum Description 3",
            Published = false,
            ParentForumId = 1,
            CategoryId = null
        };
        
        var result = await _forumService.AddForumAsync(createForumDto);
        
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(ForumErrors.NameTaken(createForumDto.Name).Message);
    }

    [Fact]
    public async Task GetForumSelectListItems_ReturnsExpectedResult()
    {
        var result = await _forumService.GetForumSelectListItemsAsync();
        var list = result.ToList();
        
        list.Count.Should().Be(2);
        list[0].Value = "1";
        list[1].Value = "2";
        list[0].Text = "Test Forum";
        list[1].Text = "Test Sub Forum";
    }
    
    [Fact]
    public async Task GetForumsAsync_ReturnsExpectedResult()
    {
        var result = await _forumService.GetForumsAsync();
        
        var list = result.ToList();
        list.Count.Should().Be(2);
    }

    [Fact]
    public async Task UpdateForumAsync_WithValidDto_ShouldUpdateForum()
    {
        var editForumDto = new EditForumDto()
        {
            Name = "Test Forum Change",
            Description = "Test Forum Description 3",
            Published = false,
            ParentForumId = null,
            CategoryId = null
        };
        
        var editResult = await _forumService.UpdateForumAsync(1, editForumDto);
        editResult.IsSuccess.Should().BeTrue();

        var updatedForum = await _forumService.GetForumAsync(1);
        updatedForum.Value.Name.Should().Be("Test Forum Change");
        updatedForum.Value.Description.Should().Be("Test Forum Description 3");
        updatedForum.Value.Published.Should().Be(false);
    }

    [Fact]
    public async Task UpdateForumAsync_WithExistingName_ReturnsFailureResult()
    {
        var editForumDto = new EditForumDto()
        {
            Name = "Test Sub Forum",
            Description = "Test Sub Forum Description 3",
            Published = false,
            ParentForumId = null,
            CategoryId = null
        };
        var editResult = await _forumService.UpdateForumAsync(1, editForumDto);
        editResult.IsSuccess.Should().BeFalse();
        editResult.Errors.Should().HaveCount(1);
        editResult.Errors[0].Message.Should().Be(ForumErrors.NameTaken(editForumDto.Name).Message);
    }

    [Fact]
    public async Task UpdateForumAsync_WithInvalidParentForumIo_ReturnsFailureResult()
    {
        var editForumDto = new EditForumDto()
        {
            Name = "Test Forum",
            Description = "Test Forum Description 3",
            Published = false,
            ParentForumId = 1,
            CategoryId = null
        };
        
        var editResult = await _forumService.UpdateForumAsync(1, editForumDto);
        editResult.IsSuccess.Should().BeFalse();
        editResult.Errors.Should().HaveCount(1);
        editResult.Errors[0].Message.Should().Be(ForumErrors.InvalidParentForumId(1).Message);
    }

    [Fact]
    public async Task GetForumForEditAsync_WithValidId_ReturnsExpectedResult()
    {
        var editForumDtoResult = await _forumService.GetForumForEditAsync(1);
        editForumDtoResult.IsSuccess.Should().BeTrue();
        
        var editForumDto = editForumDtoResult.Value;
        editForumDto.Name.Should().Be("Test Forum");
        editForumDto.Description.Should().Be("Test Forum Description");
        editForumDto.Published.Should().Be(true);
        editForumDto.CategoryId.Should().BeNull();
        editForumDto.ParentForumId.Should().BeNull();
    }

    [Fact]
    public async Task GetForumForEditAsync_WithInvalidId_ReturnsFailureResult()
    {
        var editForumDtoResult = await _forumService.GetForumForEditAsync(10);
        editForumDtoResult.IsSuccess.Should().BeFalse();
        editForumDtoResult.Errors.Should().HaveCount(1);
        editForumDtoResult.Errors[0].Message.Should().Be(ForumErrors.NotFound(10).Message);
    }
}