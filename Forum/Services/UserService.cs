using AutoMapper;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Identity;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;
using NETForum.Repositories.Filters;

namespace NETForum.Services
{
    public interface IUserService
    {
        Task<IReadOnlyCollection<User>> GetUsersAsync();
        Task<Result<User>> GetUserByIdAsync(int id);
        Task<Result<User>> GetByUsernameAsync(string userName);
        Task<bool> UserExistsAsync(int id);
        Task<EditUserDto?> GetUserForEditAsync(int id);
        Task<bool> UpdateUserRolesAsync(int userId, List<string> selectedRoleNames);
        Task<bool> UpdateUserProfileImageAsync(int userId, IFormFile file);
        Task<Result<User>> CreateUserAsync(CreateUserDto dto);
        Task<Result> CreateUserWithMemberRoleAsync(UserRegistrationDto dto);
        Task DeleteUserAsync(int id);
        Task<User?> GetNewestUserAsync();
        Task<int> GetTotalUserCountAsync();
        Task<DateTime> GetUserJoinedDate(int id);
        Task<PagedResult<User>> GetUsersPagedAsync(
            int pageNumber, 
            int pageSize, 
            UserFilterOptions userFilterOptions,
            string? sortBy,
            bool ascending
        );
    }

    public class UserService(
        UserManager<User> userManager,
        IFileStorageService fileStorageService,
        IMapper mapper,
        IUserRepository userRepository
    ) : IUserService {

        public async Task<bool> UpdateUserProfileImageAsync(int userId, IFormFile file)
        {
            var user = await GetUserByIdAsync(userId);
            if (user.IsFailure) return false;
            
            // Store for later use to delete old image
            var oldProfileImageUrl = user.Value.ProfileImageUrl;
            
            // Save the new profile image
            await using var stream = file.OpenReadStream();
            var profileImagePath = await fileStorageService.SaveFileAsync(stream, file.FileName);

            await userRepository.UpdateAsync(userId, trackedEntity =>
            {
                trackedEntity.ProfileImageUrl = profileImagePath;
            });
            
            // Delete the old profile image if it exists
            if (oldProfileImageUrl != null) 
            {
                await fileStorageService.DeleteFileAsync(oldProfileImageUrl);
            }
            
            return true;
        }

        public async Task<PagedResult<User>> GetUsersPagedAsync(
            int pageNumber, 
            int pageSize, 
            UserFilterOptions userFilterOptions,
            string? sortBy,
            bool ascending)
        {
            var repositoryPagedQueryOptions = new PagedRepositoryQueryOptions<UserFilterOptions>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                Ascending = ascending,
                Filter = userFilterOptions,
                Navigations = ["UserProfile"]
            };
            return await userRepository.GetAllPagedAsync(repositoryPagedQueryOptions);
        }

        public async Task<IReadOnlyCollection<User>> GetUsersAsync() {
            var repositoryQueryOptions = new RepositoryQueryOptions<UserFilterOptions>();
            return await userRepository.GetAllAsync(repositoryQueryOptions);
        }

        public async Task<bool> UpdateUserRolesAsync(int userId, List<string> selectedRoleNames)
        {
            var serviceResult = await GetUserByIdAsync(userId);
            if(!serviceResult.IsSuccess) return false;
            var user = serviceResult.Value;
            
            var currentRoles = await userManager.GetRolesAsync(user);
            
            // Add new roles
            var newRoles = selectedRoleNames.Where(roleName => !currentRoles.Contains(roleName));
            await userManager.AddToRolesAsync(user, newRoles);

            // Remove old roles
            var removedRoles = currentRoles
                .Where(currentRole => !selectedRoleNames.Contains(currentRole));
            await userManager.RemoveFromRolesAsync(user, removedRoles);

            return true;
        }
        
        public async Task<Result<User>> GetUserByIdAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            return user == null ? 
                Result<User>.Failure(new Error("User.NotFound", $"User with ID ${id} not found")) : 
                Result<User>.Success(user);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            var result = await userRepository.GetByIdAsync(id);
            return result != null;
        }

        public async Task<Result<User>> GetByUsernameAsync(string userName)
        {
            var navigations = new[] { "UserProfile" };
            var user = await userRepository.GetByUsernameAsync(userName, navigations);
            return user == null ?
                Result<User>.Failure(new Error("User.NotFound", $"User with name {userName} not found")) :
                Result<User>.Success(user);
        }

        public async Task<EditUserDto?> GetUserForEditAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            return user == null ? null : mapper.Map<EditUserDto>(user);
        }

        public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
        {
            var user = mapper.Map<User>(dto);
            try
            {
                await userManager.CreateAsync(user);
                var createdUser = await userManager.FindByNameAsync(dto.Username);
                return createdUser == null ? 
                    Result<User>.Failure(
                        new Error("User.NotFound", $"User with name ${dto.Username} not found")) 
                    : Result<User>.Success(user);
            }
            catch (UniqueConstraintException exception)
            {
                return Result<User>.Failure(new Error("UniqueConstraintException", exception.Message));
            }
        }

        public async Task<Result> CreateUserWithMemberRoleAsync(UserRegistrationDto dto)
        {
            var user = mapper.Map<User>(dto);
            
            var createResult = await userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                return Result.FromIdentityResult(createResult);
            }
            
            var addMemberRoleResult = await userManager.AddToRoleAsync(user, "Member");
            return !addMemberRoleResult.Succeeded ? 
                Result.FromIdentityResult(addMemberRoleResult) : 
                Result.Success();
        }

        public async Task DeleteUserAsync(int id)
        {
            await userRepository.DeleteByIdAsync(id);
        }
        
        public async Task<User?> GetNewestUserAsync()
        {
            return await userRepository.GetNewestUserAsync();
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await userRepository.CountTotalUsersAsync();
        }

        public async Task<DateTime> GetUserJoinedDate(int id)
        {
            return await userRepository.GetUserJoinedDateAsync(id);
        }
    }
}
