using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;
using NETForum.Pages.Account.Register;

namespace NETForum.Pages.Install
{
    public class IndexModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
        : PageModel
    {
        [BindProperty]
        public RegisterForm Form { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            User userDto = new User
            {
                UserName = Form.Username,
                Email = Form.Email,
                CreatedAt = DateTime.Now
            };

            var userCreateResult = await userManager.CreateAsync(userDto, Form.Password);

            // Handle errors.
            if (!userCreateResult.Succeeded)
            {
                foreach (var error in userCreateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return Page();
                }
            }

            var user = await userManager.FindByNameAsync(userDto.UserName);
            if(user == null)
            {
                // Handle erorr, user is null for some reason...
                ModelState.AddModelError("", "Could not create user. Please submit a bug");
                return Page();
            }


            var rolesString = configuration["Install:Roles"] ?? "Owner,Admin";
            var rolesArray = rolesString.Split(",").ToList() ?? new List<string>() { 
                "Admin"
            };

            var roleAddResult = await userManager.AddToRolesAsync(
                user,
                rolesArray
            );
            if (!roleAddResult.Succeeded)
            {
                foreach (var error in userCreateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return Page();
                }
            }



            // User was created and added to role "Owner".
            await signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToPage("/Index");

        }
    }
}
