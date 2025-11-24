using Ardalis.Specification;
using NETForum.Filters;
using NETForum.Models.Entities;

namespace NETForum.Services.Specifications;

public class UserProfileSearchSpec : Specification<UserProfile>
{
    public UserProfileSearchSpec(UserProfileFilterOptions filter)
    {
        // Apply filters
        if (filter.UserId.HasValue)
        {
            Query.Where(up => up.UserId == filter.UserId);
        }
        
        // Apply sorting
        switch (filter.SortBy)
        {
            case UserProfileSortBy.LastUpdated:
                if (filter.Ascending)
                {
                    Query.OrderBy(up => up.LastUpdated);
                }
                else
                {
                    Query.OrderByDescending(up => up.LastUpdated);
                }
                break;
            default:
                Query.OrderBy(up => up.Id);
                break;
        }
        
        // Apply pagination
        Query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
    }
}