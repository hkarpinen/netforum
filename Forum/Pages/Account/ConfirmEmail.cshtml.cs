using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using NETForum.Constants;
using NETForum.Services;

namespace NETForum.Pages.Account;

public class ConfirmEmailModel(IUserService userService) : PageModel
{
    public string StatusMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int userId, string? code)
    {
        if (userId == 0 || code == null)
        {
            return RedirectToPage(PageRoutes.ForumLanding);
        }

        var decodedTokenBytes = WebEncoders.Base64UrlDecode(code);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await userService.ConfirmEmailAsync(userId, decodedToken);
        StatusMessage = result.IsSuccess
            ? "Thank you for confirming your email."
            : "Error while confirming your email.";

        return Page();
    }
}