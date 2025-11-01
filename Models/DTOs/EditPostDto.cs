namespace NETForum.Pages.Posts;

public class EditPostDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int ForumId { get; set; }
    public int AuthorId { get; set; }
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }
    public bool Published { get; set; }
}