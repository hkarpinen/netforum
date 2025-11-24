namespace NETForum.Filters;

public enum UserProfileSortBy
{
    LastUpdated
}

public class UserProfileFilterOptions
{
    public int? UserId { get; set; }
    
    // Sorting options
    public UserProfileSortBy SortBy { get; set; }
    public bool Ascending { get; set; } = true;
    
    // Pagination options
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}