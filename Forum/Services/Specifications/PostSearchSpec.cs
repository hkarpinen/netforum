using Ardalis.Specification;
using NETForum.Filters;
using NETForum.Models.Entities;

namespace NETForum.Services.Specifications;

public class PostSearchSpec : Specification<Post>
{
    public PostSearchSpec(PostFilterOptions filter)
    {
        // Include author.
        Query.Include(post => post.Author);
        
        // Apply filters
        if (filter.ForumId.HasValue)
        {
            Query.Where(post => post.ForumId == filter.ForumId.Value);
        }

        if (!string.IsNullOrEmpty(filter.AuthorName))
        {
            Query.Where(post => post.Author.UserName.Contains(filter.AuthorName));
        }
        

        if (filter.Pinned.HasValue)
        {
            Query.Where(p => p.IsPinned == filter.Pinned.Value);
        }

        if (filter.Locked.HasValue)
        {
            Query.Where(p => p.IsLocked == filter.Locked.Value);
        }

        if (filter.Published.HasValue)
        {
            Query.Where(p => p.Published == filter.Published.Value);
        }

        if (!string.IsNullOrEmpty(filter.Title))
        {
            Query.Where(p => p.Title.Contains(filter.Title));
        }

        if (!string.IsNullOrEmpty(filter.Content))
        {
            Query.Where(p => p.Content.Contains(filter.Content));
        }
        
        // Apply sorting
        switch (filter.SortBy)
        {
            case PostSortBy.Title:
                if (filter.Ascending)
                {
                    Query.OrderBy(post => post.Title);
                }
                else
                {
                    Query.OrderByDescending(post => post.Title);
                }
                break;
            case PostSortBy.CreatedAt:
                if (filter.Ascending)
                {
                    Query.OrderBy(post => post.CreatedAt);
                }
                else
                {
                    Query.OrderByDescending(post => post.CreatedAt);
                }
                break;
            case PostSortBy.ViewCount:
                if (filter.Ascending)
                {
                    Query.OrderBy(post => post.ViewCount);
                }
                else
                {
                    Query.OrderByDescending(post => post.ViewCount);
                }
                break;
            case PostSortBy.ReplyCount:
                if (filter.Ascending)
                {
                    Query.OrderBy(post => post.ReplyCount);
                }
                else
                {
                    Query.OrderByDescending(post => post.ReplyCount);
                }
                break;
            default:
                Query.OrderBy(post => post.Id);
                break;
        }
        
        // Apply pagination
        Query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
    }
}