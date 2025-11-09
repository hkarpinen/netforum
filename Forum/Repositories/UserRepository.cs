using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;

namespace NETForum.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User, UserFilterOptions>(context),
    IUserRepository
{
    public override Task<User?> GetByIdAsync(int id, params string[] includes)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(u => u.Id == id);
        ApplyIncludes(query, includes);
        return query.FirstOrDefaultAsync();
    }

    public Task<User?> GetByUsernameAsync(string username, params string[] includes)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(u => u.UserName == username);
        ApplyIncludes(query, includes);
        return query.FirstOrDefaultAsync();
    }

    public async Task<User?> GetNewestUserAsync(params string[] includes)
    {
        var query = _dbSet.AsQueryable();
        query = query.OrderByDescending(u => u.CreatedAt);
        ApplyIncludes(query, includes);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<int> CountTotalUsersAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<DateTime> GetUserJoinedDateAsync(int id)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(u => u.Id == id);
        var result = await query.FirstOrDefaultAsync();
        return result.CreatedAt;
    }

    protected override IQueryable<User> ApplyFilter(IQueryable<User> query, UserFilterOptions filter)
    {
        if (!string.IsNullOrEmpty(filter.Username))
        {
            query = query.Where(u => u.UserName.Contains(filter.Username));
        }

        if (!string.IsNullOrEmpty(filter.Email))
        {
            query = query.Where(u => u.Email.Contains(filter.Email));
        }
        
        return query;
    }

    protected override IQueryable<User> ApplyIncludes(IQueryable<User> query, params string[] navigationProperties)
    {
        return navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
    }

    protected override IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "username" => ascending ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName),
            "email" => ascending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "created" => ascending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
            _ => query
        };
    }
}