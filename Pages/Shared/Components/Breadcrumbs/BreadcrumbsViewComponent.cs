using Microsoft.AspNetCore.Mvc;

namespace NETForum.Pages.Shared.Components.Breadcrumbs
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<BreadcrumbItemModel> items)
        {
            return View(items);
        }
    }
}
