using Microsoft.AspNetCore.Identity;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using FluentResults;

namespace NETForum.Services;

public class AuthenticationService(SignInManager<User> signInManager, IUserService userService) : IAuthenticationService
{
    public async Task<Result> SignInAsync(UserLoginDto  userLoginDto)
    {
        var result = await signInManager.PasswordSignInAsync(
            userLoginDto.Username, 
            userLoginDto.Password, 
            userLoginDto.RememberMe, 
            false
        );

        if (result.Succeeded) return Result.Ok();
        
        // Sign-in failed, need to the handle the errors.
        var errors = new List<Error>();
        
        if(result.IsNotAllowed) errors.Add(new Error("Email is not confirmed"));
        if(result.IsLockedOut) errors.Add(new Error("Account is locked out"));
        if(result.RequiresTwoFactor) errors.Add(new Error("Two-factor authentication is required."));
        
        // If no errors have been added, the credentials are incorrect.
        if(errors.Count == 0) errors.Add(new Error("Please verify your username and password"));
        return Result.Fail(errors);

    }

    public async Task<Result> SignInAsync(int userId, bool isPersistent = false)
    {
        var lookupResult = await userService.GetUserAsync(userId);
        if (!lookupResult.IsSuccess) return Result.Fail("User not found");
        await signInManager.SignInAsync(lookupResult.Value, isPersistent);
        return Result.Ok();
    }

    public async Task SignOutAsync()
    {
        await signInManager.SignOutAsync();
    }
}