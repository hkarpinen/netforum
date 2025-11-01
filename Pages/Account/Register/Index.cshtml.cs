using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using NETForum.Models;
using NETForum.Pages.Account.Profile;
using NETForum.Services;

namespace NETForum.Pages.Account.Register
{
    public class IndexModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserService userService,
        IUserProfileService userProfileService,
        IMapper mapper)
        : PageModel
    {
        [BindProperty]
        public UserRegistrationDto UserRegistrationDto { get; set; } = new();
        
        [BindProperty]
        public UserProfileDto UserProfileDto { get; set; } = new();

        public void OnGet()
        {
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            // Create the user and display any errors
            var user = mapper.Map<User>(UserRegistrationDto);
            
            var userCreateResult = await userManager.CreateAsync(user, UserRegistrationDto.Password);
            if(!userCreateResult.Succeeded)
            {
                foreach (var error in userCreateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return Page();
                }
            }
            

            // Add the role to new user and display any errors
            var roleAddResult = await userManager.AddToRoleAsync(user, "Member");
            if(!roleAddResult.Succeeded)
            {
                foreach (var error in roleAddResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return Page();
                }
            }
            
            try
            {
                var createdUser = await userManager.FindByNameAsync(user.UserName!);
                UserProfileDto.UserId = createdUser!.Id;
                await userProfileService.AddUserProfileAsync(UserProfileDto);
                
                // If profile image was provided, update the user's profile image
                if (UserProfileDto.ProfileImage != null)
                {
                    await userService.UpdateUserProfileImageAsync(createdUser.Id, UserProfileDto.ProfileImage);
                }
                
                // Sign in the new user and redirect.
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return Page();
            }
        }
    }
}
