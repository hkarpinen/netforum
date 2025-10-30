using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;

namespace NETForum.Pages.Account.Login
{
    public class LoginModel(
        SignInManager<User> signInManager,
        UserManager<User> userManager)
        : PageModel
    {
        private readonly UserManager<User> _userManager = userManager;

        [BindProperty]
        public LoginFormModel Form { get; set; } = new();

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
                Form.Username,
                Form.Password,
                isPersistent: Form.RememberMe,
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
