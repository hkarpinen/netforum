using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Roles
{
    [Authorize(Roles = "Admin")]
    public class CreateModel(IRoleService roleService) : PageModel
    {
        [BindProperty]
        public CreateRoleDto CreateRoleDto { get; set; } = new();

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await roleService.CreateRoleAsync(CreateRoleDto);
            
            if (result.Succeeded) return RedirectToPage("/Admin/Roles");
            
            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return Page();

        }
    }
}
