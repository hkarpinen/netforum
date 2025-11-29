using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Constants;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Admin.Members
{
    [Authorize(Roles = "Admin")]
    public class EditModel(
        IRoleService roleService,
        IUserService userService,
        IUserRoleService userRoleService
    ) : PageModel
    {
        [BindProperty]
        public EditUserDto EditUserDto { get; set; } = new();

        [BindProperty]
        public IEnumerable<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();
        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var editUserDto = await userService.GetUserForEditAsync(id);
            if(editUserDto.IsFailed) return NotFound();

            EditUserDto = editUserDto.Value;
            AllRoles = await roleService.GetSelectItemsAsync();
            SelectedRoles = await userRoleService.GetUserRoleNamesAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var result = await userService.UpdateUserRolesAsync(id, SelectedRoles.ToList());
            if(result.IsFailed) return NotFound();
            
            return RedirectToPage(PageRoutes.ManageMembers);
        }
    }
}
