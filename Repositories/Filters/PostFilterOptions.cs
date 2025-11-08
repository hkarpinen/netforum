namespace NETForum.Repositories.Filters;

public class PostFilterOptions
{
    public int? ForumId { get; set; }
    public int? AuthorId { get; set; }
    public bool? Pinned { get; set; }
    public bool? Locked { get; set; }
    public bool? Published { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
}