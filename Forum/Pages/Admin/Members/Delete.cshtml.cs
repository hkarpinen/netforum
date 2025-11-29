using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Constants;
using NETForum.Services;

namespace NETForum.Pages.Admin.Members
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel(IUserService userService) : PageModel
    {
        public async Task<IActionResult> OnGetAsync(int id)
        {
            await userService.DeleteUserAsync(id);
            return RedirectToPage(PageRoutes.ManageMembers);
        }
    }
}
