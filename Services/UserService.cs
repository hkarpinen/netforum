using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Models.DTOs;

using NETForum.Services.Criteria;

namespace NETForum.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserAsync(int id);
        Task<User?> GetUserAsync(string userName);
        Task<IdentityResult?> UpdateUserRolesAsync(User user, IEnumerable<string> selectedRoleNames);
        Task<bool> UpdateUserProfileImageAsync(int userId, IFormFile file);
        Task<IdentityResult> CreateUserAsync(CreateUserDto dto);
        Task<IdentityResult?> DeleteUserAsync(int id);
        Task<User?> GetNewestUserAsync();
        Task<int> GetTotalUserCountAsync();
        Task<DateTime> GetUserJoinedDate(int id);
        Task<PagedResult<User>> GetUsersPagedAsync(int pageNumber, int pageSize, UserSearchCriteria userSearchCriteria);
    }

    public class UserService(
        AppDbContext dbContext, 
        UserManager<User> userManager,
        IFileStorageService fileStorageService,
        IMapper mapper
    ) : IUserService {
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var result = await userManager.UpdateAsync(user);
            return result;
        }

        public async Task<bool> UpdateUserProfileImageAsync(int userId, IFormFile file)
        {
            var user = await GetUserAsync(userId);
            if (user == null) return false;
            
            // Store for later use to delete old image
            var oldProfileImageUrl = user.ProfileImageUrl;
            
            // Save the new profile image
            await using var stream = file.OpenReadStream();
            var profileImagePath = await fileStorageService.SaveFileAsync(stream, file.FileName);
            user.ProfileImageUrl = profileImagePath;
            await UpdateUserAsync(user);
            
            // Delete the old profile image if it exists
            if (oldProfileImageUrl != null) 
            {
                await fileStorageService.DeleteFileAsync(oldProfileImageUrl);
            }
            
            return true;
        }

        public async Task<PagedResult<User>> GetUsersPagedAsync(int pageNumber, int pageSize, UserSearchCriteria userSearchCriteria)
        {
            var query = dbContext.Users
                .WhereUsername(userSearchCriteria.Username)
                .WhereEmail(userSearchCriteria.Email)
                .OrderByField(userSearchCriteria.SortBy, userSearchCriteria.Ascending);
            
            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.UserProfile)
                .ToListAsync();

            return new PagedResult<User>()
            {
                Items = posts,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<User>> GetUsersAsync() {
            return await dbContext.Users
                .ToListAsync();
        }

        public async Task<IdentityResult?> UpdateUserRolesAsync(User user, IEnumerable<string> selectedRoleNames)
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

        public async Task<User?> GetUserAsync(int id)
        {
            return await dbContext.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserAsync(string userName)
        {
            return await dbContext.Users
                .Where(u => u.UserName == userName)
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync();
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserDto dto)
        {
            var user = mapper.Map<User>(dto);
            return await userManager.CreateAsync(user);
        }

        public async Task<IdentityResult?> DeleteUserAsync(int id)
        {
            IdentityResult? result = null;
            User? user = await GetUserAsync(id);
            if(user != null)
            {
                result = await userManager.DeleteAsync(user);
            }
            return result;
        }
        
        public async Task<User?> GetNewestUserAsync()
        {
            return await dbContext.Users
                .OrderByDescending(u => u.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await dbContext.Users
                .CountAsync();
        }

        public async Task<DateTime> GetUserJoinedDate(int id)
        {
            return await dbContext.Users
                .Where(u => u.Id == id)
                .Select(u => u.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
