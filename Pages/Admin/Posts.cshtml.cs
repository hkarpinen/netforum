using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NETForum.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class PostsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
