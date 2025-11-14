using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.Entities;

namespace NETForum.Services
{
    public interface IUserRoleService
    {
        Task<IEnumerable<int>?> GetUserRoleIdsAsync(string username);
        Task<IEnumerable<string>> GetUserRoleNamesAsync();
        Task<IEnumerable<string>> GetUserRoleNamesAsync(int id);
    }

    public class UserRoleService(
        AppDbContext dbContext,
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

        public async Task<IEnumerable<int>?> GetUserRoleIdsAsync(string username)
        {
            List<int>? userRoles = null;
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return userRoles;
            var roleNames = await userManager.GetRolesAsync(user);
            userRoles = await dbContext.Roles
                .Where(r => !string.IsNullOrEmpty(r.Name) && roleNames.Contains(r.Name))
                .Select(r => r.Id)
                .ToListAsync();
            return userRoles;
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int id)
        {
            IList<string>? result = null;
            var lookupResult = await userService.GetUserByIdAsync(id);
            if(lookupResult.IsSuccess)
            {
                result = await userManager.GetRolesAsync(lookupResult.Value);
            }
            return result ?? new List<string>();
        }
    }
}
