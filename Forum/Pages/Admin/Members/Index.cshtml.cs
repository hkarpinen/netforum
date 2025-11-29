using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Admin.Members
{
    [Authorize(Roles = "Admin")]
    public class IndexModel(IUserService userService) : PageModel
    {
        public IEnumerable<User>? Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await userService.GetUsersAsync();
        }
    }
}
    