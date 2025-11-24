using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NETForum.Data;
using NETForum.Services;

namespace NETForum.IntegrationTests;

public class ForumServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IForumService _forumService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMemoryCache> _mockMemoryCache;

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    public void SeedDatabase()
    {
         
    }

    public ForumServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _mockMapper = new Mock<IMapper>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        
        _context = new AppDbContext(options);
        _forumService = new ForumService(
            _mockMapper.Object,
            _context,
            _mockMemoryCache.Object
        );
    }
}