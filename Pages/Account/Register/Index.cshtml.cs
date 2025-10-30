using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Pages.Account.Profile;
using NETForum.Services;

namespace NETForum.Pages.Account.Register
{
    public class RegisterModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        UserService userService,
        IUserProfileService userProfileService)
        : PageModel
    {
        [BindProperty]
        public RegisterForm Form { get; set; } = new();
        
        [BindProperty]
        public UserProfileForm UserProfileForm { get; set; } = new();

        public void OnGet()
        {
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            // Create the user and display any errors
            var user = Form.ToNewUser();
            
            var userCreateResult = await userManager.CreateAsync(user, Form.Password);
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
                UserProfileForm.UserId = createdUser!.Id;
                await userProfileService.AddUserProfileAsync(UserProfileForm);
                
                // If profile image was provided, update the user's profile image
                if (UserProfileForm.ProfileImage != null)
                {
                    await userService.UpdateUserProfileImageAsync(createdUser.Id, UserProfileForm.ProfileImage);
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
