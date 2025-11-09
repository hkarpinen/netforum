using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories;

namespace NETForum.IntegrationTests;

public class ForumRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ForumRepository _repository;

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    public ForumRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _repository = new ForumRepository(_context);
        
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category1 = new Category() { Id = 1, Name = "Category 1" };
        var category2 = new Category() { Id = 2, Name = "Category 2" };
        _context.Categories.AddRange(category1, category2);

        var rootForum1 = new Forum()
        {
            Id = 1,
            Name = "Root Forum 1",
            Description = "Root Forum 1",
            Published = true,
            CategoryId = 1,
            ParentForumId = null,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        var rootForum2 = new Forum()
        {
            Id = 2,
            Name = "Root Forum 2",
            Description = "Root Forum 2",
            Published = true,
            CategoryId = 1,
            ParentForumId = null,
        };

        var rootForum3 = new Forum()
        {
            Id = 3,
            Name = "Unpublished Root",
            Description = "Root Forum 3",
            Published = false,
            CategoryId = 2,
            ParentForumId = null,
            CreatedAt = DateTime.UtcNow.AddDays(-3)
        };

        var subForum1 = new Forum()
        {
            Id = 4,
            Name = "Sub Forum 1",
            Description = "Sub Forum 1",
            CategoryId = 1,
            Published = true,
            ParentForumId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var subForum2 = new Forum()
        {
            Id = 5,
            Name = "Sub Forum 2",
            Description = "Sub Forum 2",
            CategoryId = 1,
            Published = true,
            ParentForumId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        
        _context.Forums.AddRange(rootForum1, rootForum2, rootForum3, subForum1, subForum2);

        var post1 = new Post()
        {
            Id = 1,
            Title = "Post 1",
            Content = "Content 1",
            ForumId = 1,
            AuthorId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var post2 = new Post()
        {
            Id = 2,
            Title = "Post 2",
            Content = "Content 2",
            ForumId = 1,
            AuthorId = 1,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Posts.AddRange(post1, post2);

        var reply1 = new Reply()
        {
            Id = 1,
            Content = "Reply 1",
            PostId = 1,
            AuthorId = 1,
            CreatedAt = DateTime.UtcNow
        };

        var reply2 = new Reply()
        {
            Id = 2,
            Content = "Reply 2",
            PostId = 1,
            AuthorId = 1,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Replies.AddRange(reply1, reply2);
        
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllRootForumsAsync_WithNoIncludes_ReturnsOnlyRootForums()
    {
        var result = await _repository.GetAllRootForumsAsync();
        result.Should().HaveCount(3);
        result.All(x => x.ParentForumId == null).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllRootForumsAsync_WithCategoryInclude_LoadsCategoryNavigation()
    {
        var navigations = new[] { "Category" };
        var result = await _repository.GetAllRootForumsAsync(navigations);
        
        result.Should().HaveCount(3);
        result.All(f => f.Category != null).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllRootForumsAsync_ExcludesSubForums()
    {
        var result = await _repository.GetAllRootForumsAsync();
        
        result.Should().NotContain(f => f.Id == 4 || f.Id == 5);
        result.Select(f => f.Name).Should().NotContain("Sub Forum 1");
    }

    [Fact]
    public async Task GetAllChildForumsAsync_ReturnsChildForums()
    {
        var result = await _repository.GetChildForumsAsync(1);
        result.Should().HaveCount(2);
        result.All(f => f.ParentForumId != null).Should().BeTrue();
    }
}