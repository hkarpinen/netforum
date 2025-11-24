using Ardalis.Specification;
using NETForum.Filters;
using NETForum.Models.Entities;

namespace NETForum.Services.Specifications;

public class ReplySearchSpec : Specification<Reply>
{
    public ReplySearchSpec(ReplyFilterOptions filter)
    {
        // Apply filters
        if (filter.PostId.HasValue)
        {
            Query.Where(x => x.PostId == filter.PostId.Value);
        }

        if (filter.AuthorId.HasValue)
        {
            Query.Where(x => x.AuthorId == filter.AuthorId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Content))
        {
            Query.Where(x => x.Content.Contains(filter.Content));
        }
        
        // Apply sorting
        switch (filter.SortBy)
        {
            case ReplySortBy.Created:
                if (filter.Ascending)
                {
                    Query.OrderBy(x => x.Id);
                }
                else
                {
                    Query.OrderByDescending(x => x.Id);
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