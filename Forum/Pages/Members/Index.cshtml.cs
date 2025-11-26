using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Filters;
using NETForum.Models.Entities;
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
    
    public PagedResult<User>? Users { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Users = await userService.GetUsersPagedAsync(new UserFilterOptions
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            Username = Username,
            SortBy = UserSortBy.CreatedAt,
            Ascending = false
        });
        
        return Page();
    }
}