using Ardalis.Specification;
using NETForum.Filters;
using NETForum.Models.Entities;

namespace NETForum.Services.Specifications;

public class RoleSearchSpec : Specification<Role>
{
    public RoleSearchSpec(RoleFilterOptions filter)
    {
        // Apply filters
        if (!string.IsNullOrEmpty(filter.Name))
        {
            Query.Where(r => r.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrEmpty(filter.Description))
        {
            Query.Where(r => r.Description.Contains(filter.Description));
        }
        
        // Apply sorting
        switch (filter.SortBy)
        {
            case RoleSortBy.Name:
                if (filter.Ascending)
                {
                    Query.OrderBy(r => r.Name);
                }
                else
                {
                    Query.OrderByDescending(r => r.Name);
                }
                break;
            default:
                Query.OrderBy(r => r.Id);
                break;
        }
        
        // Apply pagination
        Query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
    }
}