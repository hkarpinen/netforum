using AutoMapper;
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

public class UserProfileService(AppDbContext context, IMapper mapper) : IUserProfileService
{
    public async Task<Result<UserProfile>> AddUserProfileAsync(CreateUserProfileDto createUserProfileDto)
    {
        try
        {
            var userProfileExists = await context.UserProfiles.Where(p => p.UserId == createUserProfileDto.UserId).AnyAsync();
            if (userProfileExists)
            {
                return Result.Fail($"UserProfile.{createUserProfileDto.UserId}");
            }
            
            var userProfile = mapper.Map<UserProfile>(createUserProfileDto);
            var result = await context.UserProfiles.AddAsync(userProfile);
            await context.SaveChangesAsync();
            return Result.Ok(userProfile);
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
        var editUserProfileDto = mapper.Map<EditUserProfileDto>(result);
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

            mapper.Map(editUserProfileDto, userProfile);
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (DbUpdateException e)
        {
            return Result.Fail("Could not update user profile");
        }
    }
}