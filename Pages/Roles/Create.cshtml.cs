using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Roles
{
    [Authorize(Roles = "Admin")]
    public class CreateModel(IRoleService roleService, IMapper mapper) : PageModel
    {
        [BindProperty]
        public CreateRoleDto CreateRoleDto { get; set; } = new();

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            // var newRole = Form.ToNewRole();
            //var newRole = mapper.Map<Role>(Form);
            var result = await roleService.CreateRoleAsync(CreateRoleDto);

            // Handle errors
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Page();
            }

            return RedirectToPage("/Admin/Roles");
        }
    }
}
