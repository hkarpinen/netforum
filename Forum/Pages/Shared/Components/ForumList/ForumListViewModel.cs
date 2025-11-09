namespace NETForum.Pages.Shared.Components.ForumList
{
    public class ForumListViewModel
    {
        public required string Title { get; set; }
        public IEnumerable<ForumListItemModel> Forums { get; set; } = new List<ForumListItemModel>();
    }
}
