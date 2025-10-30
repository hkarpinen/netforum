namespace NETForum.Services.Criteria;

public class CategorySearchCriteria
{
    public string? Name { get; set; }
    public bool? Published { get; set; } 
    public string SortBy { get; set; }
    public bool Ascending { get; set; } = true;
}