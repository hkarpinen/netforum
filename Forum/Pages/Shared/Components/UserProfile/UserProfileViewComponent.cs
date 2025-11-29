using Microsoft.AspNetCore.Mvc;
using NETForum.Services;

namespace NETForum.Pages.Shared.Components.UserProfile
{
    public class UserProfileViewComponent(
        IUserService userService,
        IUserRoleService userRoleService,
        IFileStorageService fileStorageService)
        : ViewComponent
    {
        public async Task <IViewComponentResult> InvokeAsync() {
            if (User.Identity?.Name == null) return View();
            var lookupResult = await userService.GetUserAsync(User.Identity.Name);
            if (lookupResult.IsFailed) return View();
            var user = lookupResult.Value;
            var model = new UserProfileViewModel()
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePhotoUrl = !string.IsNullOrEmpty(user.ProfileImageUrl) ? 
                    fileStorageService.GetFileUrl(user.ProfileImageUrl) 
                    : null,
                RoleNames = await userRoleService.GetUserRoleNamesAsync(user.Id)
            };

            return View(model);
        }
    }

}
