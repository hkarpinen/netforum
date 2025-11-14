using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel(IUserService userService) : PageModel
    {
        public async Task<IActionResult> OnGetAsync(int id)
        {
            await userService.DeleteUserAsync(id);
            return RedirectToPage("/Admin/Users");
        }
    }
}
