using Microsoft.AspNetCore.Mvc;
using NETForum.Services;

namespace NETForum.Pages.Shared.Components.UserPhoto
{
    public class UserPhotoViewComponent(IFileStorageService fileStorageService) : ViewComponent
    {
        /* public IViewComponentResult Invoke(UserPhotoViewModel userPhotoViewModel)
        {
            return View(userPhotoViewModel);
        } */

        public IViewComponentResult Invoke(UserPhotoViewModel userPhotoViewModel)
        {
            string? photoUrl = null;
            if (!string.IsNullOrEmpty(userPhotoViewModel.PhotoUrl))
            {
                photoUrl = fileStorageService.GetFileUrl(userPhotoViewModel.PhotoUrl);
            }

            var model = new UserPhotoViewModel
            {
                UserName = userPhotoViewModel.UserName,
                Size = userPhotoViewModel.Size,
                PhotoUrl = photoUrl
            };
            
            return View(model);
        }
    }
}
