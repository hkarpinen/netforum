using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Attributes;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Account.Login
{
    [RedirectAuthenticatedUsers]
    public class IndexModel(IAuthenticationService authenticationService) : PageModel {
        
        [BindProperty]
        public UserLoginDto UserLoginDto { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            var loginSuccessful = await authenticationService.SignInAsync(UserLoginDto);
            if(loginSuccessful) return RedirectToPage("/Index");

            ModelState.AddModelError("", "Invalid username or password.");
            return Page();
        }
    }
}
