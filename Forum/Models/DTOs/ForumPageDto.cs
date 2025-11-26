using NETForum.Models.Entities;

namespace NETForum.Models.DTOs;

public class ForumPageDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    
    public required List<PostSummaryDto> Posts { get; set; }
    public required List<ForumListItemDto> Subforums { get; set; }
}

public class ForumListItemDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? CategoryName { get; set; }
    public int TotalPosts { get; set; }
    public int TotalReplies { get; set; }
    
    public PostSummaryDto? LastPostSummary { get; set; }
}

public class PostSummaryDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorAvatarUrl { get; set; }
    public int ReplyCount { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public ReplySummaryDto? LastReplySummary { get; set; }
}

public class PostTeaserDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorAvatarUrl { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ReplySummaryDto
{
    public int Id { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorAvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ForumIndexPageDto
{
    public required List<ForumListItemDto> RootForums { get; set; }
    public required List<PostTeaserDto> LatestPosts { get; set; }
    public User? NewestUser { get; set; }
    public HomeStatsDto? Stats { get; set; }
}

public class HomeStatsDto
{
    public int TotalPosts { get; set; }
    public int TotalMembers { get; set; }
    public int TotalReplies { get; set; }
}