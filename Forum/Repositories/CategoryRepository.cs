using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;

namespace NETForum.Repositories;

/// <summary>
/// Repository providing data access operations for Categories.
/// </summary>
/// <param name="context">DbContext instance that provides database access.</param>
public class CategoryRepository(AppDbContext context) : BaseRepository<Category, CategoryFilterOptions>(context), 
    ICategoryRepository
{
    public override async Task<Category?> GetByIdAsync(int id, params string[] includes)
    {
        var query = _dbSet.AsQueryable()
            .Where(f => f.Id == id);
        query = ApplyIncludes(query, includes);
        return await query.FirstOrDefaultAsync();
    }
    
    protected override IQueryable<Category> ApplyFilter(IQueryable<Category> query, CategoryFilterOptions criteria)
    {
        if (!string.IsNullOrEmpty(criteria.Name))
        {
            query = query.Where(c => c.Name.Contains(criteria.Name));
        }

        if (criteria.Published.HasValue)
        {
            query = query.Where(c => c.Published.Equals(criteria.Published.Value));
        }
        return query;
    }

    protected override IQueryable<Category> ApplyIncludes(IQueryable<Category> query, params string[] includeProperties)
    {
        return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    protected override IQueryable<Category> ApplySorting(IQueryable<Category> query, string sortBy, bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
            "created" => ascending ? query.OrderBy(c => c.CreatedAt)  : query.OrderByDescending(c => c.CreatedAt),
            _ => query
        };
    }
}