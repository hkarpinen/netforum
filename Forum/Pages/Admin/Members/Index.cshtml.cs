using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Filters;
using NETForum.Models;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Admin.Members
{
    [Authorize(Roles = "Admin")]
    public class IndexModel(IUserService userService) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 2;

        [BindProperty(SupportsGet = true)] 
        public UserSortBy SortBy { get; set; } = UserSortBy.CreatedAt;
        
        [BindProperty(SupportsGet = true)]
        public bool Ascending { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? UserName { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Email { get; set; }

        public PagedList<User>? Users { get; set; }

        public async Task OnGetAsync()
        {
            //Users = await userService.GetUsersAsync();
            Users = await userService.GetUsersPagedAsync(new UserFilterOptions()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Username = UserName,
                Email = Email,
                SortBy = SortBy,
                Ascending = Ascending
            });
        }
    }
}
    