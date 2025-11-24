using Microsoft.AspNetCore.Mvc;

namespace NETForum.Pages.Shared.Components.PostList;

public class PostListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(PostListViewModel model)
    {
        return View(model);
    }
}