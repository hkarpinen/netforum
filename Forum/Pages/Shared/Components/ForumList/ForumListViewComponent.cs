using Microsoft.AspNetCore.Mvc;
using NETForum.Models.DTOs;

namespace NETForum.Pages.Shared.Components.ForumList
{
   
    public class ForumListOptions
    {
        public required string ForumListTitle { get; set; }
        public required List<ForumListItemDto> ForumItems { get; set; }
    }

    public class ForumListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ForumListOptions forumListOptions)
        {
            var forumListViewModel = new ForumListViewModel
            {
                Title = forumListOptions.ForumListTitle,
                Forums = forumListOptions.ForumItems
            };

            return View(forumListViewModel);
        }
    }
}
