using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Models;
using NETForum.Pages.Account.Profile;

namespace NETForum.Services;

public interface IUserProfileService
{
    Task<EntityEntry<UserProfile>> AddUserProfileAsync(UserProfileForm userProfileForm);
    Task<UserProfile?> GetUserProfileAsync(int userId);
    Task<bool> UpdateUserProfileAsync(UserProfileForm userProfileForm);
}

public class UserProfileService(AppDbContext context) : IUserProfileService
{
    public async Task<EntityEntry<UserProfile>> AddUserProfileAsync(UserProfileForm userProfileForm)
    {
        var userProfile = new UserProfile
        {
            UserId = userProfileForm.UserId,
            Bio = userProfileForm.Bio,
            Signature = userProfileForm.Signature,
            Location = userProfileForm.Location,
            DateOfBirth = userProfileForm.DateOfBirth,
            LastUpdated = DateTime.UtcNow
        };

        var result = await context.UserProfiles.AddAsync(userProfile);
        await context.SaveChangesAsync();
        return result;
    }

    public async Task<UserProfile?> GetUserProfileAsync(int userId)
    {
        return await context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }
    
    public async Task<bool> UpdateUserProfileAsync(UserProfileForm userProfileForm)
    {
        var userProfile = await context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userProfileForm.UserId);

        if (userProfile == null) return false;

        userProfile.Bio = userProfileForm.Bio;
        userProfile.Signature = userProfileForm.Signature;
        userProfile.Location = userProfileForm.Location;
        userProfile.DateOfBirth = userProfileForm.DateOfBirth;
        userProfile.LastUpdated = DateTime.UtcNow;

        context.UserProfiles.Update(userProfile);
        await context.SaveChangesAsync();
        return true;
    }
}