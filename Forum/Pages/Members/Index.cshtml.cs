using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Filters;
using NETForum.Models;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Members;

public class IndexModel(IUserService userService) : PageModel
{
    [FromQuery]
    public UserFilterOptions Filter { get; set; } = new();
    
    public PagedList<User>? Users { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Users = await userService.GetUsersPagedAsync(Filter);
        return Page();
    }
}