namespace NETForum.Models.DTOs;

public class UserPageDto
{
    public string? Username { get; set; }
    public string? AvatarImageUrl { get; set; }
    public DateTime JoinedOn { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string? Signature { get; set; }

    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    
    public List<ReplyViewModel> Replies { get; set; }
    public List<PostSummaryDto> Posts { get; set; }
}