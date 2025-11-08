namespace NETForum.Repositories.Filters;

/// <summary>
/// Filter options for querying Categories
/// </summary>
public class CategoryFilterOptions
{
    public string? Name { get; set; }
    public bool? Published { get; set; } 
}