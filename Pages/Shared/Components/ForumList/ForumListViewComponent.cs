using Microsoft.AspNetCore.Mvc;

namespace NETForum.Pages.Shared.Components.ForumList
{
   
    public class ForumListOptions
    {
        public required string ForumListTitle { get; set; }
        public required IEnumerable<ForumListItemModel> ForumItems { get; set; }
    }

    public class ForumListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ForumListOptions forumListOptions)
        {
            ForumListViewModel forumListViewModel = new ForumListViewModel
            {
                Title = forumListOptions.ForumListTitle,
                Forums = forumListOptions.ForumItems
            };

            return View(forumListViewModel);
        }
    }
}
