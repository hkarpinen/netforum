using NETForum.Models.Entities;

namespace NETForum.Models.Components
{
    public class PostListItem : Post
    {
        public Reply? LastReply { get; set; }
        // public string? ProfilePictureUrl { get; set; }
    }
}
