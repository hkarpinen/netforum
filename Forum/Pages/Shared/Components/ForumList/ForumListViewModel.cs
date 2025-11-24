using NETForum.Models.DTOs;

namespace NETForum.Pages.Shared.Components.ForumList
{
    public class ForumListViewModel
    {
        public required string Title { get; set; }
        public List<ForumListItemDto> Forums { get; set; } = new();
    }
}
