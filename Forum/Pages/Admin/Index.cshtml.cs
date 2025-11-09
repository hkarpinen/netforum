using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NETForum.Pages.Admin
{
    [Authorize(Roles = "Owner,Admin")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
