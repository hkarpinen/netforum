using AutoMapper;
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
        Task<User?> GetUserAsync(int id);
        Task<User?> GetUserAsync(string userName);
        Task<IdentityResult?> UpdateUserRolesAsync(User user, List<string> selectedRoleNames);
        Task<bool> UpdateUserProfileImageAsync(int userId, IFormFile file);
        Task<IdentityResult> CreateUserAsync(CreateUserDto dto);
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
            var user = await GetUserAsync(userId);
            if (user == null) return false;
            
            // Store for later use to delete old image
            var oldProfileImageUrl = user.ProfileImageUrl;
            
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

        public async Task<IdentityResult?> UpdateUserRolesAsync(User user, List<string> selectedRoleNames)
        {
            var currentRoles = await userManager.GetRolesAsync(user);
            
            // Add new roles
            var newRoles = selectedRoleNames.Where(roleName => !currentRoles.Contains(roleName));
            var result = await userManager.AddToRolesAsync(user, newRoles);

            // Remove old roles
            var removedRoles = currentRoles
                .Where(currentRole => !selectedRoleNames.Contains(currentRole));
            var removeResult = userManager.RemoveFromRolesAsync(user, removedRoles);

            return result;
        }

        // TODO: Rename to GetUserByIdAsync()
        public async Task<User?> GetUserAsync(int id)
        {
            return await userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserAsync(string userName)
        {
            var navigations = new[] { "UserProfile" };
            var user = await userRepository.GetByUsernameAsync(userName, navigations);
            return user;
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserDto dto)
        {
            var user = mapper.Map<User>(dto);
            return await userManager.CreateAsync(user);
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
