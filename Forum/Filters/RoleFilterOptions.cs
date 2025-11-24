namespace NETForum.Filters;

public enum RoleSortBy
{
    Name
}

public class RoleFilterOptions
{
    public required string? Name { get; set; }
    public required string? Description { get; set; }
    
    // Sorting options
    public RoleSortBy SortBy { get; set; }
    public bool Ascending { get; set; } = true;
    
    // Pagination options
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}