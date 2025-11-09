using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel(
        IRoleService roleService,
        IUserService userService,
        IUserRoleService userRoleService,
        IMapper mapper
    ) : PageModel
    {
        [BindProperty]
        public EditUserDto EditUserDto { get; set; } = new();

        [BindProperty]
        public IEnumerable<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();
        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await userService.GetUserAsync(id);
            if (user == null) return NotFound();

            // Populate form dropdowns
            EditUserDto = mapper.Map<EditUserDto>(user);
            AllRoles = await roleService.GetSelectItemsAsync();
            SelectedRoles = await userRoleService.GetUserRoleNamesAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await userService.GetUserAsync(id);
            if (user == null) return NotFound();
            await userService.UpdateUserRolesAsync(user, SelectedRoles.ToList());
            return RedirectToPage("/Admin/Users");
        }
    }
}
