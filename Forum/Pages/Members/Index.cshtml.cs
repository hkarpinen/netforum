using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Pages.Members;

public class IndexModel(IUserService userService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    
    [BindProperty(SupportsGet = true)] 
    public int PageSize { get; set; } = 10;
    
    [BindProperty(SupportsGet = true)]
    public string? Username { get; set; }
    
    public PagedResult<User> Users { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Users = await userService.GetUsersPagedAsync(
            PageNumber,
            PageSize,
            new UserFilterOptions()
            {
                Username = Username
            }, "created", false
        );
        return Page();
    }
}