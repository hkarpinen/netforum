using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;
using NETForum.Services;

namespace NETForum.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel(IUserService userService) : PageModel
    {
        public IEnumerable<User>? Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await userService.GetUsersAsync();
        }
    }
}
    