using NETForum.Models.DTOs;

namespace NETForum.Pages.Shared.Components.PostList;

public class PostListViewModel
{
    public required List<PostSummaryDto> Posts { get; set; }
}