using Ardalis.Specification;
using NETForum.Filters;
using NETForum.Models.Entities;

namespace NETForum.Services.Specifications;

public class UserSearchSpec : Specification<User>
{
    public UserSearchSpec(UserFilterOptions filter)
    {
        // Apply filters
        if (!string.IsNullOrEmpty(filter.Username))
        {
            Query.Where(x => x.UserName != null && x.UserName.Contains(filter.Username));
        }

        if (!string.IsNullOrEmpty(filter.Email))
        {
            Query.Where(x => x.Email != null && x.Email.Contains(filter.Email));
        }
        
        // Apply sorting
        switch (filter.SortBy)
        {
            case UserSortBy.Email:
                if (filter.Ascending)
                {
                    Query.OrderBy(x => x.Email);
                }
                else
                {
                    Query.OrderByDescending(x => x.Email);
                }
                break;
            case UserSortBy.Username:
                if (filter.Ascending)
                {
                    Query.OrderBy(x => x.UserName);
                }
                else
                {
                    Query.OrderByDescending(x => x.UserName);
                }
                break;
            case UserSortBy.CreatedAt:
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