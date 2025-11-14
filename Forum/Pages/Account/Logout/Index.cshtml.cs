using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Account.Logout
{
    public class LogoutModel(IAuthenticationService authenticationService) : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await authenticationService.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
