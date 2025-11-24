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
        public CreateUserProfileDto CreateUserProfileDto { get; set; } = new();
        
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();
            
            var userCreateResult = await userService.CreateUserWithMemberRoleAsync(UserRegistrationDto);
            if (!userCreateResult.IsSuccess)
            {
                foreach (var error in userCreateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                return Page();
            }
            
            var createdUser = await userService.GetByUsernameAsync(UserRegistrationDto.Username);
            if (!createdUser.IsSuccess)
            {
                foreach (var error in createdUser.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                return Page();
            }

            // Update the user profile DTO with the newly created User record ID.
            CreateUserProfileDto.UserId = createdUser.Value.Id;

            var profileAddResult = await userProfileService.AddUserProfileAsync(CreateUserProfileDto);
            if (!profileAddResult.IsSuccess)
            {
                foreach (var error in profileAddResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                return Page();
            }

            if (CreateUserProfileDto.ProfileImage != null)
            {
                var profileImageAddResult = await userService.UpdateUserProfileImageAsync(createdUser.Value.Id, CreateUserProfileDto.ProfileImage);
                if (profileImageAddResult.IsFailed)
                {
                    foreach (var error in profileImageAddResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Message);
                    }
                    return Page();
                }
            }

            // Sign in the new user and redirect.
            await authenticationService.SignInAsync(createdUser.Value.Id);
            return RedirectToPage("/Index");
            
        }
    }
}
