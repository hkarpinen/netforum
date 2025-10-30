using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Services;

namespace NETForum.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel(
        IRoleService roleService,
        IUserService userService,
        IUserRoleService userRoleService)
        : PageModel
    {
        [BindProperty]
        public UserForm Form { get; set; } = new();
        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await userService.GetUserAsync(id);
            if (user == null) return NotFound();

            // Populate form dropdowns
            Form = user.ToForm();
            Form.AvailableRoles = await roleService.GetSelectItemsAsync();
            Form.SelectedRoleNames = await userRoleService.GetUserRoleNamesAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await userService.GetUserAsync(id);
            if (user == null) return NotFound();
            await userService.UpdateUserRolesAsync(user, Form.SelectedRoleNames);
            return RedirectToPage("/Admin/Users");
        }
    }
}
