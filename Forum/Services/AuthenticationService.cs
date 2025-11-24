using Microsoft.AspNetCore.Identity;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using FluentResults;

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
        if (!lookupResult.IsSuccess) return Result.Fail("User not found");
        await signInManager.SignInAsync(lookupResult.Value, isPersistent);
        return Result.Ok();
    }

    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();
    }
}