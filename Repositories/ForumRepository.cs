using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;

namespace NETForum.Repositories;

/// <summary>
/// Repository providing data access operations for Forums
/// </summary>
/// <param name="context">DbContext instance that provides database access.</param>
public class ForumRepository(AppDbContext context) : BaseRepository<Forum, ForumFilterOptions>(context), 
    IForumRepository
{
    public async Task<IReadOnlyCollection<Forum>> GetAllRootForumsAsync(params string[] includes)
    {
        var query = _dbSet.AsQueryable().Where(f => f.ParentForumId == null);
        query = ApplyIncludes(query, includes);
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyCollection<Forum>> GetForumPostsWithReplies(int forumId)
    {
        return await _dbSet.Where(f => f.ParentForumId == forumId)
            .Include(f => f.Posts)
            .ThenInclude(f => f.Replies)
            .ToListAsync();
    }

    public override async Task<Forum?> GetByIdAsync(int id, params string[] includes)
    {
        var query = _dbSet.AsQueryable()
            .Where(f => f.Id == id);
        query = ApplyIncludes(query, includes);
        return await query.FirstOrDefaultAsync();
    }

    protected override IQueryable<Forum> ApplyFilter(IQueryable<Forum> query, ForumFilterOptions filter)
    {
        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(c => c.Name.Contains(filter.Name));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == filter.CategoryId);
        }

        if (filter.Published.HasValue)
        {
            query = query.Where(c => c.Published == filter.Published);
        }

        if (filter.ParentForumId.HasValue)
        {
            query = query.Where(c => c.ParentForumId == filter.ParentForumId);
        }

        return query;
    }

    protected override IQueryable<Forum> ApplyIncludes(IQueryable<Forum> query, params string[] navigationProperties)
    {
        return navigationProperties.Aggregate(query,
            (current, navigationProperty) => current.Include(navigationProperty));
    }

    protected override IQueryable<Forum> ApplySorting(IQueryable<Forum> query, string sortBy, bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "created" => ascending ? query.OrderBy(c => c.CreatedAt) :  query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Name),
        };
    }
}