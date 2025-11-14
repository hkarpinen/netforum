using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;

namespace NETForum.Repositories;

public class UserProfileRepository(AppDbContext context) : BaseRepository<UserProfile, UserProfileFilterOptions>(context),
    IUserProfileRepository
{
    public override async Task<UserProfile?> GetByIdAsync(int id, params string[] includes)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.Id == id);
        ApplyIncludes(query, includes);
        return await query.FirstOrDefaultAsync();
    }

    protected override IQueryable<UserProfile> ApplyFilter(IQueryable<UserProfile> query, UserProfileFilterOptions filter)
    {
        if (filter.UserId.HasValue)
        {
            query = query.Where(p => p.UserId == filter.UserId);
        }
        return query;
    }

    protected override IQueryable<UserProfile> ApplyIncludes(IQueryable<UserProfile> query, params string[] navigationProperties)
    {
        return navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
    }

    protected override IQueryable<UserProfile> ApplySorting(IQueryable<UserProfile> query, string sortBy, bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "last_updated" => ascending
                ? query.OrderBy(p => p.LastUpdated)
                : query.OrderByDescending(p => p.LastUpdated),
            _ => query
        };
    }

    public async Task<UserProfile?> GetByUserIdAsync(int userId, params string[] navigations)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.UserId == userId);
        ApplyIncludes(query, navigations);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> UserProfileExists(int userId)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.UserId == userId);
        var result = await query.AnyAsync();
        return result;
    }
}