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
            var result = await userService.DeleteUserAsync(id);
            
            // User not found
            if(result == null)
            {
                return NotFound();
            }

            // Handle errors
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Page();
            }

            return RedirectToPage("/Admin/Users");
        }
    }
}
