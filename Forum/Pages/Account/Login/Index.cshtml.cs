using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Attributes;
using NETForum.Constants;
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
            if(loginSuccessful.IsSuccess) return RedirectToPage(PageRoutes.ForumLanding);

            foreach (var error in loginSuccessful.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }
    }
}
