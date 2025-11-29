using NETForum.Models.Entities;

namespace NETForum.IntegrationTests;

using Microsoft.AspNetCore.Identity;

public class FakeTokenProvider : IUserTwoFactorTokenProvider<User>
{
    public Task<string> GenerateAsync(string purpose, UserManager<User> manager, User user)
    {
        return Task.FromResult("fake-token-123");
    }

    public Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
    {
        return Task.FromResult(token == "fake-token-123");
    }

    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user)
    {
        return Task.FromResult(true);
    }
}