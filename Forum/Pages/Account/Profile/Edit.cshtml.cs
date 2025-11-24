using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Account.Profile
{
    [Authorize(Roles = "Admin,Member")]
    public class EditModel(IUserService userService, IUserProfileService userProfileService) : PageModel
    {
        [BindProperty] 
        public EditUserProfileDto EditUserProfileDto { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if(User.Identity?.Name == null) return RedirectToPage("/Account/Login");
            var userLookupResult = await userService.GetByUsernameAsync(User.Identity.Name);
            if (userLookupResult.IsFailed) return NotFound();
            var user = userLookupResult.Value;
            
            // Populate the form with existing profile data
            var editUserProfileDtoResult = await userProfileService.GetUserProfileForEditAsync(user.Id);
            if (editUserProfileDtoResult.IsFailed) return NotFound();
            EditUserProfileDto = editUserProfileDtoResult.Value;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (User.Identity?.Name == null) return RedirectToPage("/Account/Login");
            var lookupResult = await userService.GetByUsernameAsync(User.Identity.Name);
            if (lookupResult.IsFailed) return NotFound();
            var user = lookupResult.Value;
            
            // Do any other necessary profile updates here (e.g., updating bio, signature, etc.)
            await userProfileService.UpdateUserProfileAsync(user.Id, EditUserProfileDto);

            // Handle profile image upload if a new image is provided
            if (EditUserProfileDto.ProfileImage != null)
            {
                await userService.UpdateUserProfileImageAsync(user.Id, EditUserProfileDto.ProfileImage);
            }
            
            return RedirectToPage("/Index");
        }
    }
}