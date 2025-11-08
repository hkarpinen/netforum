namespace NETForum.Repositories.Filters;

public class ReplyFilterOptions
{
    public int? PostId { get; set; }
    public int? AuthorId { get; set; }
    public string? Content { get; set; }
}