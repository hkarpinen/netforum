using Microsoft.AspNetCore.Identity;
using NETForum.Data;
using NETForum.Models;

namespace NETForum.Services
{
    public interface IInstallService
    {
        Task<bool> IsInstallCompleteAsync();
    }

    public class InstallService(UserManager<User> userManager) : IInstallService
    {
        public async Task<bool> IsInstallCompleteAsync()
        {
            try
            {
                var ownerUsers = await userManager.GetUsersInRoleAsync("Owner");
                return ownerUsers.Any();
            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}
