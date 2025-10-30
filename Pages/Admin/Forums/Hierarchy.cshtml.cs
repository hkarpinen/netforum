using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;
using NETForum.Services;

namespace NETForum.Pages.Admin.Forums
{
    public class HierarchyModel(IForumService forumService) : PageModel
    {
        public IEnumerable<Forum>? Forums;

        public async Task OnGetAsync()
        {
            Forums = await forumService.GetForumsAsync();
        }
    }
}
