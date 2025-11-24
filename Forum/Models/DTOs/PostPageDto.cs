namespace NETForum.Models.DTOs;

public class PostPageDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int AuthorId { get; set; }
    public string AuthorAvatarImageUrl { get; set; }
    public string AuthorName { get; set; }
    public List<ReplyViewModel> Replies { get; set; }
    public AuthorStatsSummary AuthorStatsSummary { get; set; }
    public bool CurrentUserIsAuthor { get; set; }
}

public class AuthorStatsSummary
{
    public int TotalPostCount { get; set; }
    public int TotalReplyCount { get; set; }
    public DateTime JoinedOn { get; set; }
}

public class ReplyViewModel
{
    public int Id { get; set; }
    public string AuthorName { get; set; }
    public string? AuthorAvatarImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; }
}