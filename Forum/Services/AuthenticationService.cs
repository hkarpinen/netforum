using Microsoft.AspNetCore.Identity;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;

namespace NETForum.Services;

public class AuthenticationService(SignInManager<User> signInManager, IUserService userService) : IAuthenticationService
{
    public async Task<bool> SignInAsync(UserLoginDto  userLoginDto)
    {
        var result = await signInManager.PasswordSignInAsync(
            userLoginDto.Username, 
            userLoginDto.Password, 
            userLoginDto.RememberMe, 
            false
        );
        return result.Succeeded;
    }

    public async Task<Result> SignInAsync(int userId, bool isPersistent = false)
    {
        var lookupResult = await userService.GetUserByIdAsync(userId);
        if (!lookupResult.IsSuccess) return lookupResult;
        await signInManager.SignInAsync(lookupResult.Value, isPersistent);
        return Result.Success();
    }

    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();
    }
}