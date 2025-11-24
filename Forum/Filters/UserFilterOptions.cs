namespace NETForum.Filters;

public enum UserSortBy
{
    Username,
    Email,
    CreatedAt
}

public class UserFilterOptions
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    
    // Sorting options
    public UserSortBy SortBy { get; set; }
    public bool Ascending { get; set; } = true;
    
    // Pagination options
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}