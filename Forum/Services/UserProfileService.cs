using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;

namespace NETForum.Services;

public interface IUserProfileService
{
    Task<Result<UserProfile>> AddUserProfileAsync(UserProfileDto userProfileDto);
    Task<UserProfile?> GetUserProfileAsync(int userId);
    Task<bool> UpdateUserProfileAsync(UserProfileDto userProfileDto);
}

public class UserProfileService(AppDbContext context) : IUserProfileService
{
    public async Task<Result<UserProfile>> AddUserProfileAsync(UserProfileDto userProfileDto)
    {
        try
        {
            var userProfileExists = await context.UserProfiles.AnyAsync(x => x.UserId == userProfileDto.UserId);
            if (userProfileExists)
            {
                return Result<UserProfile>.Failure(new Error("UserProfile.AlreadyExists", $"UserProfile.{userProfileDto.UserId}"));
            }
        
            // Create the object to insert into the database.
            var userProfile = new UserProfile
            {
                UserId = userProfileDto.UserId,
                Bio = userProfileDto.Bio,
                Signature = userProfileDto.Signature,
                Location = userProfileDto.Location,
                DateOfBirth = userProfileDto.DateOfBirth,
                LastUpdated = DateTime.UtcNow
            };

            // TODO: User profiles service needs a UserProfileRepository class instead so it does not directly use db context.
            var result = await context.UserProfiles.AddAsync(userProfile);
            await context.SaveChangesAsync();
            return Result<UserProfile>.Success(result.Entity);
        }
        catch (Exception ex)
        {
            return Result<UserProfile>.Failure(new Error("Internal error", ex.Message));
        }
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