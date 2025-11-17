using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;

namespace NETForum.Repositories;

public class RoleRepository(AppDbContext context) : BaseRepository<Role, RoleFilterOptions>(context),
    IRoleRepository
{
    public override Task<Role?> GetByIdAsync(int id, params string[] includes)
    {
        var query = _dbSet.AsNoTracking();
        query = ApplyIncludes(query, includes);
        return query.FirstOrDefaultAsync(r => r.Id == id);
    }

    protected override IQueryable<Role> ApplyFilter(IQueryable<Role> query, RoleFilterOptions filter)
    {
        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(r => r.Name.Contains(filter.Name));
        }
        
        if(!string.IsNullOrEmpty(filter.Description))
        {
            query = query.Where(r => r.Description.Contains(filter.Description));
        }
        return query;
    }

    protected override IQueryable<Role> ApplyIncludes(IQueryable<Role> query, params string[] navigationProperties)
    {
        return navigationProperties.Aggregate(query, (current, navigationProperty) => current.Include(navigationProperty));
    }

    protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, string sortBy, bool ascending)
    {
        return sortBy.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            _ => query
        };
    }
}