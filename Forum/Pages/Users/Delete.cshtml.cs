using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel(IUserService userService) : PageModel
    {
        public async Task<IActionResult> OnGetAsync(int id) {
            try
            {
                await userService.DeleteUserAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            return RedirectToPage("/Admin/Users");
        }
    }
}
