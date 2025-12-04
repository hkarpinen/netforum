using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETForum.Models.Entities;

namespace NETForum.Services
{
    public interface IUserRoleService
    {
        Task<IEnumerable<string>> GetUserRoleNamesAsync(int id);
    }

    public class UserRoleService(
        IUserService userService,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
        : IUserRoleService
    {
        public async Task<IEnumerable<string>> GetUserRoleNamesAsync()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return roles.Select(r => r.Name).Where(name => name != null).ToList()!;
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int id)
        {
            IList<string>? result = null;
            var lookupResult = await userService.GetUserAsync(id);
            if(lookupResult.IsSuccess)
            {
                result = await userManager.GetRolesAsync(lookupResult.Value);
            }
            return result ?? new List<string>();
        }
    }
}
