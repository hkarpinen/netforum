using NETForum.Models.DTOs;
using FluentResults;

namespace NETForum.Services;

public interface IAuthenticationService
{
    Task<Result> SignInAsync(UserLoginDto userLoginDto);
    Task<Result> SignInAsync(int userId, bool isPersistent = false);
    Task SignOutAsync();
}