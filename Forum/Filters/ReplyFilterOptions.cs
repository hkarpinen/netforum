namespace NETForum.Filters;

public enum ReplySortBy
{
    Created
}

public class ReplyFilterOptions
{
    public int? PostId { get; set; }
    public int? AuthorId { get; set; }
    public string? Content { get; set; }
    
    // Sorting options
    public ReplySortBy SortBy { get; set; }
    public bool Ascending { get; set; }
    
    // Pagination options
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}