using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel(
        IRoleService roleService,
        IUserService userService) : PageModel
    {
        [BindProperty]
        public CreateUserDto CreateUserDto { get; set; } = new();
        
        [BindProperty]
        public IEnumerable<string> SelectedRoles { get; set; } = new List<string>();
        
        public IEnumerable<SelectListItem> RoleSelectListItems { get; set; }  = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            RoleSelectListItems = await roleService.GetSelectItemsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await userService.CreateUserAsync(CreateUserDto);
            
            // If adding the user succeeded, update the roles for the user.
            if (result.IsSuccess)
            {
                var user = await userService.GetByUsernameAsync(CreateUserDto.Username);
                if(user.IsFailure) return NotFound();
                
                var rolesToList = SelectedRoles.ToList();
                var updateRolesResult = await userService.UpdateUserRolesAsync(user.Value.Id, rolesToList);
                if (!updateRolesResult)
                {
                    // TODO: Should gracefully fail here.
                }
                return RedirectToPage("/Admin/Users");
            }
            
            // Handle the model error.
            ModelState.AddModelError(result.Error.Code, result.Error.Message);
            return Page();
        } 
    }
}
