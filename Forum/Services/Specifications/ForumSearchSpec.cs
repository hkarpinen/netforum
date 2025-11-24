using Ardalis.Specification;
using NETForum.Filters;
using NETForum.Models.Entities;

namespace NETForum.Services.Specifications;

public class ForumSearchSpec : Specification<Forum>
{
    public ForumSearchSpec(ForumFilterOptions filter)
    {
        Query.Include(x => x.Category);
        Query.Include(x => x.ParentForum);
        
        // Apply filters
        if (!string.IsNullOrEmpty(filter.Name))
        {
            Query.Where(x => x.Name.Contains(filter.Name));
        }

        if (filter.CategoryId.HasValue)
        {
            Query.Where(x => x.CategoryId == filter.CategoryId);
        }

        if (filter.Published.HasValue)
        {
            Query.Where(x => x.Published == filter.Published);
        }

        if (filter.ParentForumId.HasValue)
        {
            Query.Where(x => x.ParentForumId == filter.ParentForumId);
        }
        
        // Apply sorting
        switch (filter.SortBy)
        {
            case ForumSearchSortBy.Name:
                if (filter.Ascending)
                {
                    Query.OrderBy(x => x.Name);
                }
                else
                {
                    Query.OrderByDescending(x => x.Name);
                }
                break;
            case ForumSearchSortBy.CreatedAt:
                if (filter.Ascending)
                {
                    Query.OrderBy(x => x.CreatedAt);
                }
                else
                {
                    Query.OrderByDescending(x => x.CreatedAt);
                }
                break;
            default:
                Query.OrderBy(x => x.Id);
                break;
        }
        
        // Apply pagination
        Query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
    }
}