using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        [BindProperty]
        public CreateUserDto CreateUserDto { get; set; } = new();
        
        [BindProperty]
        public IEnumerable<string> SelectedRoles { get; set; } = new List<string>();
        
        public IEnumerable<SelectListItem> Roles { get; set; }  = new List<SelectListItem>();

        public CreateModel(
            IRoleService roleService,
            RoleManager<Role> roleManager,
            IUserService userService
        ) {
            _roleService = roleService;
            _userService = userService;
        }
        
        public async Task OnGetAsync()
        {
            Roles = await _roleService.GetSelectItemsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await _userService.CreateUserAsync(CreateUserDto);
            
            // If adding the user succeeded, update the roles for the user.
            if (result.Succeeded)
            {
                var user = await _userService.GetUserAsync(CreateUserDto.Username);
                if (user != null)
                {
                    await _userService.UpdateUserRolesAsync(user, SelectedRoles.ToList());
                }
            }
            
            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return RedirectToPage("/Admin/Users");
        } 
    }
}
