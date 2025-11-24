namespace NETForum.Filters
{
    public enum ForumSearchSortBy
    {
        Name,
        CreatedAt
    }
    
    public class ForumFilterOptions
    {
        public string? Name {  get; set; }
        public int? CategoryId { get; set; }
        public int? ParentForumId { get; set; }
        public bool? Published { get; set; }
        
        // Sorting options
        public ForumSearchSortBy SortBy { get; set; }
        public bool Ascending { get; set; } = true;
        
        // Pagination options
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
