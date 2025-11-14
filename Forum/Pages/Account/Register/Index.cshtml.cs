using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Account.Register
{
    public class IndexModel(
        IUserService userService,
        IAuthenticationService authenticationService,
        IUserProfileService userProfileService
    ) : PageModel {
        
        [BindProperty]
        public UserRegistrationDto UserRegistrationDto { get; set; } = new();
        
        [BindProperty]
        public UserProfileDto UserProfileDto { get; set; } = new();
        
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();
            
            var userCreateResult = await userService.CreateUserWithMemberRoleAsync(UserRegistrationDto);
            if (!userCreateResult.IsSuccess)
            {
                ModelState.AddModelError(userCreateResult.Error.Code, userCreateResult.Error.Message);
                return Page();
            }
            
            var createdUser = await userService.GetByUsernameAsync(UserRegistrationDto.Username);
            if (!createdUser.IsSuccess)
            {
                ModelState.AddModelError(createdUser.Error.Code, createdUser.Error.Message);
                return Page();
            }

            // Update the user profile DTO with the newly created User record ID.
            UserProfileDto.UserId = createdUser.Value.Id;

            var profileAddResult = await userProfileService.AddUserProfileAsync(UserProfileDto);
            if (!profileAddResult.IsSuccess)
            {
                ModelState.AddModelError(userCreateResult.Error.Code, userCreateResult.Error.Message);
                return Page();
            }

            if (UserProfileDto.ProfileImage != null)
            {
                var profileImageAddResult = await userService.UpdateUserProfileImageAsync(createdUser.Value.Id, UserProfileDto.ProfileImage);
                if (!profileImageAddResult)
                {
                    ModelState.AddModelError("", "Could not add profile image");
                    return Page();
                }
            }

            // Sign in the new user and redirect.
            await authenticationService.SignInAsync(createdUser.Value.Id);
            return RedirectToPage("/Index");
            
        }
    }
}
