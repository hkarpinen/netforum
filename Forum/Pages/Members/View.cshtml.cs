using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Members;

public class ViewModel(IUserService userService) : PageModel
{
    public UserPageDto? UserPageDto { get; set; }
    
    public async Task<IActionResult> OnGetAsync(string username)
    {
        var userPageDtoResult = await userService.GetUserPageDtoAsync(username);
        if (userPageDtoResult.IsFailed) return NotFound();
        UserPageDto = userPageDtoResult.Value;
        return Page();
    }
}