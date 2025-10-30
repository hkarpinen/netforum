namespace NETForum.Services.Criteria;

public class PostSearchCriteria
{
    public int? ForumId { get; set; }
    public int? AuthorId { get; set; }
    public bool? Pinned { get; set; }
    public bool? Locked { get; set; }
    public bool? Published { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string SortBy { get; set; }
    public bool Ascending { get; set; } = true;
}