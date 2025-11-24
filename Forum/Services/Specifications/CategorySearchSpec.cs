using Ardalis.Specification;
using NETForum.Filters;
using NETForum.Models.Entities;

namespace NETForum.Services.Specifications;

public class CategorySearchSpec : Specification<Category>
{
    public CategorySearchSpec(CategoryFilterOptions filter)
    {
        // Apply includes
        Query.Include(c => c.Forums);
        
        // Apply filters
        if (!string.IsNullOrEmpty(filter.Name))
        {
            Query.Where(c => c.Name.Contains(filter.Name));
        }

        if (filter.Published.HasValue)
        {
            Query.Where(c => c.Published == filter.Published);
        }
        
        // Apply sorting
        switch (filter.SortBy)
        {
            case CategorySortBy.Name:
                if (filter.Ascending)
                {
                    Query.OrderBy(c => c.Name);
                }
                else
                {
                    Query.OrderByDescending(c => c.Name);
                }
                break;
            case CategorySortBy.CreatedAt:
                if (filter.Ascending)
                {
                    Query.OrderBy(c => c.CreatedAt);
                }
                else
                {
                    Query.OrderByDescending(c => c.CreatedAt);
                }
                break;
            default:
                Query.OrderBy(c => c.Id);
                break;
        }
        
        // Apply pagination
        Query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
    }
}