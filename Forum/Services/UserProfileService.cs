using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;

namespace NETForum.Services;

public interface IUserProfileService
{
    Task<EntityEntry<UserProfile>> AddUserProfileAsync(UserProfileDto userProfileDto);
    Task<UserProfile?> GetUserProfileAsync(int userId);
    Task<bool> UpdateUserProfileAsync(UserProfileDto userProfileDto);
}

public class UserProfileService(AppDbContext context) : IUserProfileService
{
    public async Task<EntityEntry<UserProfile>> AddUserProfileAsync(UserProfileDto userProfileDto)
    {
        var userProfile = new UserProfile
        {
            UserId = userProfileDto.UserId,
            Bio = userProfileDto.Bio,
            Signature = userProfileDto.Signature,
            Location = userProfileDto.Location,
            DateOfBirth = userProfileDto.DateOfBirth,
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
    
    public async Task<bool> UpdateUserProfileAsync(UserProfileDto userProfileDto)
    {
        var userProfile = await context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userProfileDto.UserId);

        if (userProfile == null) return false;

        userProfile.Bio = userProfileDto.Bio;
        userProfile.Signature = userProfileDto.Signature;
        userProfile.Location = userProfileDto.Location;
        userProfile.DateOfBirth = userProfileDto.DateOfBirth;
        userProfile.LastUpdated = DateTime.UtcNow;

        context.UserProfiles.Update(userProfile);
        await context.SaveChangesAsync();
        return true;
    }
}