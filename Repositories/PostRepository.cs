using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;

namespace NETForum.Repositories;

public class PostRepository(AppDbContext context) : BaseRepository<Post, PostFilterOptions>(context),
    IPostRepository
{
    public override Task<Post?> GetByIdAsync(int id, params string[] includes)
    {
        var query = _dbSet.AsQueryable();
        ApplyIncludes(query, includes);
        return query.FirstOrDefaultAsync(p => p.Id == id);
    }

    protected override IQueryable<Post> ApplyFilter(IQueryable<Post> query, PostFilterOptions filter)
    {
        if (filter.ForumId.HasValue)
        {
            query = query.Where(p => p.ForumId == filter.ForumId.Value);
        }

        if (filter.AuthorId.HasValue)
        {
            query = query.Where(p => p.AuthorId == filter.AuthorId.Value);
        }

        if (filter.Pinned.HasValue)
        {
            query = query.Where(p => p.IsPinned == filter.Pinned.Value);
        }

        if (filter.Locked.HasValue)
        {
            query = query.Where(p => p.IsLocked == filter.Locked.Value);
        }

        if (filter.Published.HasValue)
        {
            query = query.Where(p => p.Published == filter.Published.Value);
        }

        if (!string.IsNullOrEmpty(filter.Title))
        {
            query = query.Where(p => p.Title.Contains(filter.Title));
        }

        if (!string.IsNullOrEmpty(filter.Content))
        {
            query = query.Where(p => p.Content.Contains(filter.Content));
        }

        return query;
    }

    protected override IQueryable<Post> ApplyIncludes(IQueryable<Post> query, params string[] navigationProperties)
    {
        return navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
    }

    protected override IQueryable<Post> ApplySorting(IQueryable<Post> query, string sortBy, bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "title" => ascending ? query.OrderBy(p => p.Title) : query.OrderByDescending(p => p.Title),
            "created" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            "views" => ascending ? query.OrderBy(p => p.ViewCount) : query.OrderByDescending(p => p.ViewCount),
            "replies" => ascending ? query.OrderBy(p => p.ReplyCount) : query.OrderByDescending(p => p.ReplyCount),
            _ => query.OrderByDescending(p => p.IsPinned).ThenByDescending(p => p.CreatedAt)
        };
    }

    public async Task<IReadOnlyCollection<Post>> GetPostsInForumAsync(int forumId, params string[] navigations)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.ForumId == forumId);
        ApplyIncludes(query, navigations);
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyCollection<Post>> GetLatestPostsAsync(int limit, params string[] navigations)
    {
        var query = _dbSet.AsQueryable();
        ApplyIncludes(query, navigations);
        return await query.OrderByDescending(p => p.UpdatedAt).Take(limit).ToListAsync();
    }
    
    public async Task<int> GetTotalPostCountByAuthorAsync(int authorId)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.AuthorId == authorId);
        return await query.CountAsync();
    }

    public async Task<int> GetTotalPostCountAllTime()
    {
        var query = _dbSet.AsQueryable();
        return await query.CountAsync();
    }
}