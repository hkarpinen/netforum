using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Constants;

namespace NETForum.Pages
{
    public class IndexModel(ILogger<IndexModel> logger) : PageModel
    {

        public bool UserIsAdmin = false;
        public bool UserIsAuthenticated = false; 

        private readonly ILogger<IndexModel> _logger = logger;

        public IActionResult OnGet()
        {
            return RedirectToPage(PageRoutes.ForumLanding);
        }
    }
}
