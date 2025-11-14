using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Members;

public class ViewModel(IUserService userService) : PageModel
{
    public new User User { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync(string username)
    {
        var lookupResult = await userService.GetByUsernameAsync(username);
        if (lookupResult.IsFailure) return NotFound();
        User = lookupResult.Value;
        return Page();
    }
}