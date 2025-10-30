using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;
using NETForum.Services;

namespace NETForum.Pages.Members;

public class ViewModel(IUserService userService) : PageModel
{
    public new User User { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync(string username)
    {
        var result = await userService.GetUserAsync(username);
        if (result == null) return NotFound();
        User = result;
        return Page();
    }
}