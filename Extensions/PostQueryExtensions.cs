namespace NETForum.Extensions;

public static class PostQueryExtensions
{
    public static IQueryable<Models.Post> WhereForum(this IQueryable<Models.Post> query, int? forumId)
    {
        return forumId.HasValue ? query.Where(p => p.ForumId == forumId) : query;
    }
    
    public static IQueryable<Models.Post> WhereAuthor(this IQueryable<Models.Post> query, int? authorId)
    {
        return authorId.HasValue ? query.Where(p => p.AuthorId == authorId) : query;
    }
    
    public static IQueryable<Models.Post> WherePinned(this IQueryable<Models.Post> query, bool? isPinned)
    {
        return isPinned.HasValue ? query.Where(p => p.IsPinned == isPinned) : query;
    }
    
    public static IQueryable<Models.Post> WhereLocked(this IQueryable<Models.Post> query, bool? isLocked)
    {
        return isLocked.HasValue ? query.Where(p => p.IsLocked == isLocked) : query;
    }
    
    public static IQueryable<Models.Post> WherePublished(this IQueryable<Models.Post> query, bool? published)
    {
        return published.HasValue ? query.Where(p => p.Published == published) : query;
    }
    
    public static IQueryable<Models.Post> WhereTitle(this IQueryable<Models.Post> query, string? title)
    {
        return !string.IsNullOrEmpty(title) ? query.Where(p => p.Title.Contains(title)) : query;
    }
    
    public static IQueryable<Models.Post> WhereContent(this IQueryable<Models.Post> query, string? content)
    {
        return !string.IsNullOrEmpty(content) ? query.Where(p => p.Content.Contains(content)) : query;
    }
    
    public static IQueryable<Models.Post> OrderByField(this IQueryable<Models.Post> query, string sortBy, bool ascending)
    {
        return sortBy?.ToLower() switch
        {
            "title" => ascending ? query.OrderBy(p => p.Title) : query.OrderByDescending(p => p.Title),
            "created" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            "views" => ascending ? query.OrderBy(p => p.ViewCount) : query.OrderByDescending(p => p.ViewCount),
            "replies" => ascending ? query.OrderBy(p => p.ReplyCount) : query.OrderByDescending(p => p.ReplyCount),
            _ => query.OrderByDescending(p => p.IsPinned).ThenByDescending(p => p.CreatedAt)
        };
    }
}