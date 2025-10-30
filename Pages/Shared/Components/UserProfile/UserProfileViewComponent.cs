using Microsoft.AspNetCore.Mvc;
using NETForum.Models;
using NETForum.Services;

namespace NETForum.Pages.Shared.Components.UserProfile
{
    public class UserProfileViewComponent(
        IUserService userService,
        IUserRoleService userRoleService,
        IFileStorageService fileStorageService)
        : ViewComponent
    {
        public async Task <IViewComponentResult> InvokeAsync(
            
        ) {
            if(User.Identity == null)
            {
                return View();
            }

            if(User.Identity.Name == null)
            {
                return View();
            }

            User? user = await userService.GetUserAsync(User.Identity.Name);

            if(user == null)
            {
                return View();
            }
            
            string? photoUrl = null;
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                photoUrl = fileStorageService.GetFileUrl(user.ProfileImageUrl);
            }

            var model = new UserProfileViewModel()
            {
                Id = user.Id,
                DisplayName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePhotoUrl = photoUrl,
                RoleNames = await userRoleService.GetUserRoleNamesAsync(user.Id)
            };

            return View(model);
        }
    }

}
