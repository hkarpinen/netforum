using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.Entities;

namespace NETForum.Pages.Account.Logout
{
    public class LogoutModel(SignInManager<User> signInManager) : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}
