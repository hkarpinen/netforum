namespace NETForum.Filters;

public enum CategorySortBy
{
    Name,
    CreatedAt
}

public class CategoryFilterOptions
{
    public string? Name { get; set; }
    public bool? Published { get; set; } 
    
    // Sorting options
    public CategorySortBy SortBy { get; set; }
    public bool Ascending { get; set; } = true;
    
    // Pagination 
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;

}