using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;

namespace NETForum.Repositories;

public class ReplyRepository(AppDbContext context) : BaseRepository<Reply, ReplyFilterOptions>(context),
    IReplyRepository
{
    public async Task<IReadOnlyCollection<Reply>> GetRepliesByPostAsync(int postId, params string[] navigations)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.PostId == postId);
        query = ApplyIncludes(query, navigations);
        return await query.ToListAsync();
    }

    public async Task<int> GetTotalReplyCountAsync()
    {
        var query = _dbSet.AsQueryable();
        return await query.CountAsync();
    }

    public Task<int> GetTotalReplyCountByAuthorAsync(int authorId)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.AuthorId == authorId);
        return query.CountAsync();
    }

    public override Task<Reply?> GetByIdAsync(int id, params string[] includes)
    {
        var query = _dbSet.AsQueryable();
        query = query.Where(p => p.Id == id);
        query = ApplyIncludes(query, includes);
        return query.FirstOrDefaultAsync();
    }

    protected override IQueryable<Reply> ApplyFilter(IQueryable<Reply> query, ReplyFilterOptions filter)
    {
        if (filter.PostId.HasValue)
        {
            query = query.Where(p => p.PostId == filter.PostId.Value);
        }

        if (filter.AuthorId.HasValue)
        {
            query = query.Where(p => p.AuthorId == filter.AuthorId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Content))
        {
            query = query.Where(p => p.Content.Contains(filter.Content));
        }

        return query;
    }

    protected override IQueryable<Reply> ApplyIncludes(IQueryable<Reply> query, params string[] navigationProperties)
    {
        return navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
    }

    protected override IQueryable<Reply> ApplySorting(IQueryable<Reply> query, string sortBy, bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "created" => ascending ? query.OrderBy(r => r.CreatedAt) : query.OrderByDescending(r => r.CreatedAt),
            _ => query
        };
    }
}