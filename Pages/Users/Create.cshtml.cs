using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Services;

namespace NETForum.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        [BindProperty]
        public UserForm Form { get; set; } = new();

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
            Form.AvailableRoles = await _roleService.GetSelectItemsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var newUser = Form.ToNewUser();
            var result = await _userService.CreateUserAsync(newUser);
            if (result.Succeeded) return RedirectToPage("/Admin/Users");
            
            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return RedirectToPage("/Admin/Users");
        } 
    }
}
