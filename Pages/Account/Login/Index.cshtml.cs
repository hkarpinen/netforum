using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;

namespace NETForum.Pages.Account.Login
{
    public class IndexModel(
        SignInManager<User> signInManager)
        : PageModel
    {
        [BindProperty]
        public UserLoginDto UserLoginDto { get; set; } = new();

        public IActionResult OnGet()
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                return RedirectToPage("/Index");
            }

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(User.Identity is { IsAuthenticated: true })
            {
                return RedirectToPage("/Index");
            }

            var result = await signInManager.PasswordSignInAsync(
                UserLoginDto.Username,
                UserLoginDto.Password,
                isPersistent: UserLoginDto.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError("", "Invalid Username or password");

            return Page();
        }
    }
}
