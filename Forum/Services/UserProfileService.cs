using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using FluentResults;

namespace NETForum.Services;

public interface IUserProfileService
{
    Task<Result<UserProfile>> AddUserProfileAsync(CreateUserProfileDto createUserProfileDto);
    Task<Result<UserProfile>> GetUserProfileAsync(int userId);
    Task<Result<EditUserProfileDto>> GetUserProfileForEditAsync(int userId);
    Task<Result> UpdateUserProfileAsync(int userId, EditUserProfileDto editUserProfileDto);
}

public class UserProfileService(AppDbContext context) : IUserProfileService
{
    public async Task<Result<UserProfile>> AddUserProfileAsync(CreateUserProfileDto createUserProfileDto)
    {
        try
        {
            var userProfileExists = await context.UserProfiles.Where(p => p.UserId == createUserProfileDto.UserId).AnyAsync();
            if (userProfileExists)
            {
                return Result.Fail<UserProfile>($"UserProfile.{createUserProfileDto.UserId}");
            }
            
            // Map Create DTO to UserProfile
            var userProfile = new UserProfile
            {
                Bio = createUserProfileDto.Bio,
                Signature = createUserProfileDto.Signature,
                Location = createUserProfileDto.Location,
                DateOfBirth = createUserProfileDto.DateOfBirth,
                LastUpdated = DateTime.UtcNow
            };
            
            var result = await context.UserProfiles.AddAsync(userProfile);
            await context.SaveChangesAsync();
            return Result.Ok(result.Entity);
        }
        catch (DbUpdateException e)
        {
            return Result.Fail<UserProfile>("Could not add user profile");
        }
    }

    public async Task<Result<UserProfile>> GetUserProfileAsync(int userId)
    {
        var result = await context.UserProfiles.Where(p => p.UserId == userId).FirstOrDefaultAsync();
        return result == null ?
            Result.Fail<UserProfile>("Could not find user profile") : 
            Result.Ok(result);
    }

    public async Task<Result<EditUserProfileDto>> GetUserProfileForEditAsync(int userId)
    {
        var result = await context.UserProfiles.Where(p => p.UserId == userId).FirstOrDefaultAsync();
        if (result == null)
        {
            return Result.Fail<EditUserProfileDto>("Could not find user profile");
        }
        
        // Map UserProfile to Edit DTO
        var editUserProfileDto = new EditUserProfileDto
        {
            Bio = result.Bio,
            Signature = result.Signature,
            Location = result.Location,
            DateOfBirth = result.DateOfBirth,
        };
        
        return Result.Ok(editUserProfileDto);
    }
    
    public async Task<Result> UpdateUserProfileAsync(int userId, EditUserProfileDto editUserProfileDto)
    {
        try
        {
            var userProfile = await context.UserProfiles.Where(p => p.UserId == userId).FirstOrDefaultAsync();
            if (userProfile == null)
            {
                return Result.Fail("Could not find user profile");
            }

            // Map Edit DTO to UserProfile
            userProfile.Bio = editUserProfileDto.Bio;
            userProfile.Signature = editUserProfileDto.Signature;
            userProfile.Location = editUserProfileDto.Location;
            userProfile.DateOfBirth = editUserProfileDto.DateOfBirth;
            
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (DbUpdateException e)
        {
            return Result.Fail("Could not update user profile");
        }
    }
}