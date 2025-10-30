using NETForum.Models;

namespace NETForum.Pages.Shared.Components.ForumList
{
    public class ForumListItemModel
    {
        public Forum Forum { get; set; }
        public int PostCount { get; set; }
        public int RepliesCount { get; set; }
        public Post? LastPost { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
