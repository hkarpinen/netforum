namespace NETForum.Filters;

public enum PostSortBy
{
    Title,
    CreatedAt,
    ViewCount,
    ReplyCount
}

public class PostFilterOptions
{
    public int? ForumId { get; set; }
    public int? AuthorId { get; set; }
    public bool? Pinned { get; set; }
    public bool? Locked { get; set; }
    public bool? Published { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    
    // Sorting options
    public PostSortBy SortBy { get; set; }
    public bool Ascending { get; set; } = true;
    
    // Pagination options
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}