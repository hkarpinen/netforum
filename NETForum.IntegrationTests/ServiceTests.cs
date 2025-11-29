using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;

namespace NETForum.IntegrationTests;

public class ServiceTests : IDisposable
{
    protected readonly AppDbContext _db;
    private readonly SqliteConnection _connection;

    public ServiceTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .UseExceptionProcessor()
            .Options;
        
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _db.Database.EnsureDeleted();
        _connection.Dispose();
    }
}