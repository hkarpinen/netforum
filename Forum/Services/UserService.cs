using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services.Specifications;
using FluentResults;

namespace NETForum.Services
{
    public interface IUserService
    {
        Task<IReadOnlyCollection<User>> GetUsersAsync();
        Task<Result<User>> GetUserByIdAsync(int id);
        Task<Result<User>> GetByUsernameAsync(string userName);
        Task<bool> UserExistsAsync(int id);
        Task<EditUserDto?> GetUserForEditAsync(int id);
        Task<Result> UpdateUserRolesAsync(int userId, List<string> selectedRoleNames);
        Task<Result> UpdateUserProfileImageAsync(int userId, IFormFile file);
        Task<Result<User>> CreateUserAsync(CreateUserDto dto);
        Task<Result> CreateUserWithMemberRoleAsync(UserRegistrationDto dto);
        Task<IdentityResult?> DeleteUserAsync(int id);
        Task<PagedResult<User>> GetUsersPagedAsync(UserFilterOptions userFilterOptions);
    }

    public class UserService(
        UserManager<User> userManager,
        IFileStorageService fileStorageService,
        IMapper mapper,
        AppDbContext appDbContext
    ) : IUserService {

        public async Task<Result> UpdateUserProfileImageAsync(int userId, IFormFile file)
        {
            try
            {
                var user = await appDbContext.Users.FindAsync(userId);
                if (user == null) return Result.Fail("User not found");
            
                // Store for later use to delete old image
                var oldProfileImageUrl = user.ProfileImageUrl;
            
                // Save the new profile image
                await using var stream = file.OpenReadStream();
                var profileImagePath = await fileStorageService.SaveFileAsync(stream, file.FileName);

                user.ProfileImageUrl = profileImagePath;
                await appDbContext.SaveChangesAsync();
            
                // Delete the old profile image if it exists
                if (oldProfileImageUrl != null) 
                {
                    await fileStorageService.DeleteFileAsync(oldProfileImageUrl);
                }
            
                return Result.Ok();
            }
            catch (DbUpdateException)
            {
                return Result.Fail("Could not update profile image.");
            }
        }

        public async Task<PagedResult<User>> GetUsersPagedAsync(UserFilterOptions userFilterOptions)
        {
            var userSearchSpec = new UserSearchSpec(userFilterOptions);

            var totalUsers = await appDbContext.Users.CountAsync();
            var users = await appDbContext.Users
                .WithSpecification(userSearchSpec)
                .ToListAsync();
            var pagedResult = new PagedResult<User>
            {
                Items = users,
                PageNumber = userFilterOptions.PageNumber,
                PageSize = userFilterOptions.PageSize,
                TotalCount = totalUsers
            };
            return pagedResult;
        }

        public async Task<IReadOnlyCollection<User>> GetUsersAsync() {
            return await appDbContext.Users.ToListAsync();
        }

        public async Task<Result> UpdateUserRolesAsync(int userId, List<string> selectedRoleNames)
        {
            var serviceResult = await GetUserByIdAsync(userId);
            if (!serviceResult.IsSuccess) return Result.Ok();
            var user = serviceResult.Value;
            
            var currentRoles = await userManager.GetRolesAsync(user);
            
            // Add new roles
            var newRoles = selectedRoleNames.Where(roleName => !currentRoles.Contains(roleName));
            await userManager.AddToRolesAsync(user, newRoles);

            // Remove old roles
            var removedRoles = currentRoles
                .Where(currentRole => !selectedRoleNames.Contains(currentRole));
            await userManager.RemoveFromRolesAsync(user, removedRoles);

            return Result.Ok();
        }
        
        public async Task<Result<User>> GetUserByIdAsync(int id)
        {
            var user = await appDbContext.Users.FindAsync(id);
            return user == null ? 
                Result.Fail<User>($"User with ID ${id} not found") : 
                Result.Ok(user);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await appDbContext.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<Result<User>> GetByUsernameAsync(string userName)
        {
            var user = await appDbContext.Users
                .Include(x => x.UserProfile)
                .FirstOrDefaultAsync(u => u.UserName == userName);
            return user == null ?
                Result.Fail<User>($"User with name {userName} not found") :
                Result.Ok(user);
        }

        public async Task<EditUserDto?> GetUserForEditAsync(int id)
        {
            var user = await appDbContext.Users.FindAsync(id);
            return user == null ? null : mapper.Map<EditUserDto>(user);
        }

        public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
        {
            var user = mapper.Map<User>(dto);
            try
            {
                await userManager.CreateAsync(user);
                var createdUser = await userManager.FindByNameAsync(dto.Username);
                return createdUser == null
                    ? Result.Fail<User>($"User with name ${dto.Username} not found")
                    : Result.Ok(user);
            }
            catch (UniqueConstraintException exception)
            {
                var constraintShortName = exception.ConstraintName.Split("_").Last();
                return Result.Fail<User>($"{constraintShortName} is already used.");
            }
        }

        public async Task<Result> CreateUserWithMemberRoleAsync(UserRegistrationDto dto)
        {
            var user = mapper.Map<User>(dto);
            
            var createResult = await userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                return Result.Ok();
            }
            
            var addMemberRoleResult = await userManager.AddToRoleAsync(user, "Member");

            return Result.FailIfNotEmpty(
                addMemberRoleResult.Errors.Select(e => new Error(e.Description)));
        }

        public async Task<IdentityResult?> DeleteUserAsync(int id)
        {
            var user = await appDbContext.Users.FindAsync(id);
            if(user == null) return null;
            return await userManager.DeleteAsync(user);
        }
    }
}
