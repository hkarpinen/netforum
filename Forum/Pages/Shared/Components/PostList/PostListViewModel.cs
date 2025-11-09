using Microsoft.AspNetCore.Mvc;
using NETForum.Models.Entities;

namespace NETForum.Pages.Shared.Components.PostList;

public class PostListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IEnumerable<Post> posts)
    {
        return View(posts);
    }
}