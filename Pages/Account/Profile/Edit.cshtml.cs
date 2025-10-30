using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Account.Profile
{
    [Authorize(Roles = "Admin,Member")]
    public class EditModel(IUserService userService, IUserProfileService userProfileService) : PageModel
    {
        [BindProperty] 
        public UserProfileForm UserProfileForm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if(User.Identity?.Name == null) return RedirectToPage("/Account/Login");
            var user = await userService.GetUserAsync(User.Identity.Name);
            if (user == null) return NotFound();
            
            UserProfileForm.UserId = user.Id;
            var userProfile = await userProfileService.GetUserProfileAsync(user.Id);

            if (userProfile == null) return Page();
            
            // Populate the form with existing profile data
            UserProfileForm.Bio = userProfile.Bio;
            UserProfileForm.Signature = userProfile.Signature;
            UserProfileForm.Location = userProfile.Location;
            UserProfileForm.DateOfBirth = userProfile.DateOfBirth;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (User.Identity?.Name == null) return RedirectToPage("/Account/Login");
            var user = await userService.GetUserAsync(User.Identity.Name);
            if (user == null) return NotFound();
            
            // Do any other necessary profile updates here (e.g., updating bio, signature, etc.)
            await userProfileService.UpdateUserProfileAsync(UserProfileForm);

            // Handle profile image upload if a new image is provided
            if (UserProfileForm.ProfileImage != null)
            {
                await userService.UpdateUserProfileImageAsync(user.Id, UserProfileForm.ProfileImage);
            }
            
            return RedirectToPage("/Index");
        }
    }
}