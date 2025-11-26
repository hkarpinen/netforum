using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Roles
{
    [Authorize(Roles = "Admin")]
    public class EditModel(IRoleService roleService) : PageModel
    {
        [BindProperty]
        public EditRoleDto EditRoleDto { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var roleEditDtoResult = await roleService.GetRoleForEditAsync(id);
            if(roleEditDtoResult.IsFailed) return NotFound();
            EditRoleDto = roleEditDtoResult.Value;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var roleEditDtoResult = await roleService.GetRoleForEditAsync(id);
            if(roleEditDtoResult.IsFailed) return NotFound();
            EditRoleDto = roleEditDtoResult.Value;
            
            var result = await roleService.UpdateRoleAsync(id, EditRoleDto);
            
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
