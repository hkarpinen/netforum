using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Services;

namespace NETForum.Pages.Roles
{
    [Authorize(Roles = "Admin")]
    public class EditModel(IRoleService roleService) : PageModel
    {
        [BindProperty]
        public RoleForm Form { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            var role = await roleService.GetRoleAsync(id);
            if(role != null)
            {
                Form = role.ToForm();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var role = await roleService.GetRoleAsync(id);
            if (role == null) return NotFound();
            
            var result = await roleService.UpdateRoleAsync(
                Form.MapToRole(role)    
            );
            
            // TODO: An error occurred, NotFound() is not the correct object to return here.
            if (result == null) return NotFound();
            if (result.Succeeded) return RedirectToPage("/Admin/Roles");
            
            // Display errors on failure.
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            
            return Page();
        }
    }
}
