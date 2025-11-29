using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Constants;
using NETForum.Services;

namespace NETForum.Pages.Admin.Roles
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel(IRoleService roleService) : PageModel
    {
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await roleService.DeleteRoleAsync(id);
            
            // No role was found.
            if(result == null)
            {
                return NotFound();
            }

            // Display errors
            if(!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    return Page();
                }
            }

            return RedirectToPage(PageRoutes.ManageRoles);
        }
    }
}
