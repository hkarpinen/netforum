using AutoMapper;
using NETForum.Data;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;

namespace NETForum.Services;

public interface IUserProfileService
{
    Task<Result<UserProfile>> AddUserProfileAsync(CreateUserProfileDto createUserProfileDto);
    Task<Result<UserProfile>> GetUserProfileAsync(int userId);
    Task<Result<EditUserProfileDto>> GetUserProfileForEditAsync(int userId);
    Task<Result> UpdateUserProfileAsync(int userId, EditUserProfileDto editUserProfileDto);
}

public class UserProfileService(AppDbContext context, IUserProfileRepository userProfileRepository, IMapper mapper) : IUserProfileService
{
    public async Task<Result<UserProfile>> AddUserProfileAsync(CreateUserProfileDto createUserProfileDto)
    {
        try
        {
            var userProfileExists = await userProfileRepository.UserProfileExists(createUserProfileDto.UserId);
            if (userProfileExists)
            {
                return Result<UserProfile>.Failure(new Error("UserProfile.AlreadyExists", $"UserProfile.{createUserProfileDto.UserId}"));
            }
            
            var userProfile = mapper.Map<UserProfile>(createUserProfileDto);
            var result = await userProfileRepository.AddAsync(userProfile);
            await context.SaveChangesAsync();
            return Result<UserProfile>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<UserProfile>.Failure(new Error("Internal error", ex.Message));
        }
    }

    public async Task<Result<UserProfile>> GetUserProfileAsync(int userId)
    {
        var result = await userProfileRepository.GetByUserIdAsync(userId);
        return result == null ?
            Result<UserProfile>.Failure(new Error("UserProfile.NotFound", $"UserProfile.{userId}")) : 
            Result<UserProfile>.Success(result);
    }

    public async Task<Result<EditUserProfileDto>> GetUserProfileForEditAsync(int userId)
    {
        var result = await userProfileRepository.GetByUserIdAsync(userId);
        if (result == null)
        {
            return Result<EditUserProfileDto>.Failure(new Error("UserProfile.NotFound", $"UserProfile.{userId}"));
        }
        var editUserProfileDto = mapper.Map<EditUserProfileDto>(result);
        return Result<EditUserProfileDto>.Success(editUserProfileDto);
    }
    
    public async Task<Result> UpdateUserProfileAsync(int userId, EditUserProfileDto editUserProfileDto)
    {
        var userProfile = await userProfileRepository.GetByUserIdAsync(userId);
        if (userProfile == null)
        {
            return Result.Failure(new Error("UserProfile.NotFound", $"UserProfile.{userId}"));
        }
        await userProfileRepository.UpdateAsync(userProfile.Id, trackedUserProfile =>
        {
            mapper.Map(editUserProfileDto, trackedUserProfile);
        });
        return Result.Success();
    }
}