using System.Text;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services.Specifications;
using FluentResults;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using NETForum.Errors;
using NETForum.Models;

namespace NETForum.Services
{
    public interface IUserService
    {
        Task<IReadOnlyCollection<User>> GetUsersAsync();
        Task<Result<User>> GetUserAsync(int id);
        Task<Result<UserPageDto>> GetUserPageDtoAsync(string username);
        Task<Result<User>> GetUserAsync(string userName);
        Task<Result<string>> GenerateEmailConfirmationUrlAsync(string userName);
        Task<Result> ConfirmEmailAsync(int userId, string token);
        Task<Result<EditUserDto>> GetUserForEditAsync(int id);
        Task<Result> UpdateUserRolesAsync(int userId, List<string> selectedRoleNames);
        Task<Result> UpdateUserProfileImageAsync(int userId, IFormFile file);
        Task<Result<User>> CreateUserAsync(CreateUserDto dto);
        Task<Result> RegisterUserAsync(UserRegistrationDto dto);
        Task<Result> DeleteUserAsync(int id);
        Task<PagedList<User>> GetUsersPagedAsync(UserFilterOptions userFilterOptions);
    }

    public class UserService(
        UserManager<User> userManager,
        IFileStorageService fileStorageService,
        AppDbContext appDbContext,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        IEmailSender emailSender,
        ILogger<UserService> logger
    ) : IUserService {

        public async Task<Result> ConfirmEmailAsync(int userId, string token)
        {
            var user = await appDbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return Result.Fail(UserErrors.NotFound(userId));
            }
            
            var result = await userManager.ConfirmEmailAsync(user, token);
            
            return !result.Succeeded ? 
                Result.Fail(result.Errors.Select(e => e.Description)) : 
                Result.Ok();
        }
        
        public async Task<Result<string>> GenerateEmailConfirmationUrlAsync(string userName)
        {
            var user = await appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if(user == null) return Result.Fail(UserErrors.NotFound(userName));
            
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Result.Fail("Cannot generate link: No active HTTP context");
            }

            var url = linkGenerator.GetUriByPage(
                httpContext,
                page: "/Account/ConfirmEmail",
                handler: null,
                values: new { userId = user.Id, code = encodedToken }
            );

            return string.IsNullOrEmpty(url) ? 
                Result.Fail<string>("Failed to generate confirmation URL.") : 
                Result.Ok(url);
        }

        public async Task<Result<UserPageDto>> GetUserPageDtoAsync(string username)
        {
            var dto = await appDbContext.Users
                .Where(u => u.UserName == username)
                .Select(u => new UserPageDto
                {
                    Username = u.UserName,
                    AvatarImageUrl = u.ProfileImageUrl,
                    JoinedOn = u.CreatedAt,
                    Location = u.UserProfile.Location,
                    Bio = u.UserProfile.Bio,
                    Signature = u.UserProfile.Signature,
                    Replies = u.Replies.Select(r => new ReplyViewModel
                    {
                        Id = r.Id,
                        AuthorName = r.Author.UserName,
                        AuthorAvatarImageUrl = r.Author.ProfileImageUrl,
                        CreatedAt = r.CreatedAt,
                        PostTitle = r.Post.Title,
                        Content = r.Content
                    }).ToList(),
                    Posts = u.Posts.Select(p => new PostSummaryDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorName = p.Author.UserName,
                        AuthorAvatarUrl = p.Author.ProfileImageUrl,
                        ReplyCount = p.Replies.Count,
                        ViewCount = p.Replies.Count,
                        CreatedAt = p.CreatedAt,
                        LastReplySummary = p.Replies.OrderByDescending(p => p.CreatedAt)
                            .Select(r => new ReplySummaryDto()
                            {
                                Id = r.Id,
                                AuthorName = r.Author.UserName,
                                AuthorAvatarUrl = r.Author.ProfileImageUrl,
                                CreatedAt = r.CreatedAt,
                            }).FirstOrDefault()
                    }).ToList()
                }).FirstOrDefaultAsync();
            
            return dto == null ?
                Result.Fail<UserPageDto>(UserErrors.NotFound(username)) :
                Result.Ok(dto);
        }

        public async Task<Result> UpdateUserProfileImageAsync(int userId, IFormFile file)
        {
            try
            {
                var user = await appDbContext.Users.FindAsync(userId);
                if (user == null) return Result.Fail(UserErrors.NotFound(userId));
            
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
                return Result.Fail(UserErrors.DbUpdateFailed(userId));
            }
        }

        public async Task<PagedList<User>> GetUsersPagedAsync(UserFilterOptions userFilterOptions)
        {
            var userSearchSpec = new UserSearchSpec(userFilterOptions);

            var totalUsers = await appDbContext.Users.CountAsync();
            var users = await appDbContext.Users
                .WithSpecification(userSearchSpec)
                .ToListAsync();
            var pagedList = new PagedList<User>(
                users,
                totalUsers,
                userFilterOptions.PageNumber,
                userFilterOptions.PageSize
            );
            return pagedList;
        }

        public async Task<IReadOnlyCollection<User>> GetUsersAsync() {
            return await appDbContext.Users.ToListAsync();
        }

        public async Task<Result> UpdateUserRolesAsync(int userId, List<string> selectedRoleNames)
        {
            var user = await appDbContext.Users.FindAsync(userId);
            if (user == null) return Result.Fail(UserErrors.NotFound(userId));
            
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
        
        public async Task<Result<User>> GetUserAsync(int id)
        {
            var user = await appDbContext.Users.FindAsync(id);
            return user == null ? 
                Result.Fail<User>(UserErrors.NotFound(id)) : 
                Result.Ok(user);
        }

        public async Task<Result<User>> GetUserAsync(string userName)
        {
            var user = await appDbContext.Users
                .Include(x => x.UserProfile)
                .FirstOrDefaultAsync(u => u.UserName == userName);
            return user == null ?
                Result.Fail<User>(UserErrors.NotFound(userName)) :
                Result.Ok(user);
        }

        public async Task<Result<EditUserDto>> GetUserForEditAsync(int id)
        {
            var user = await appDbContext.Users.FindAsync(id);
            if (user == null) return Result.Fail<EditUserDto>(UserErrors.NotFound(id));
            
            // Map User to Edit DTO
            var editUserDto = new EditUserDto
            {
                Email = user.Email,
                Username = user.UserName
            };
            
            return Result.Ok(editUserDto);
        }

        public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
        {
            // Username is taken
            if (await appDbContext.Users.AnyAsync(u => u.UserName == dto.Username))
            {
                return Result.Fail<User>(UserErrors.NameTaken(dto.Username));
            }
            
            try
            {
                // Map Create DTO to user
                var user = new User
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = false
                };
                
                await userManager.CreateAsync(user);
                var createdUser = await userManager.FindByNameAsync(dto.Username);
                return createdUser == null
                    ? Result.Fail<User>(UserErrors.NotFound(dto.Username))
                    : Result.Ok(user);
            }
            catch (Exception ex)
            {
                return Result.Fail<User>(ex.Message);
            }
        }

        public async Task<Result> RegisterUserAsync(UserRegistrationDto dto)
        {
            
            // Map User Registration DTO to User
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            var createResult = await userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                return Result.FailIfNotEmpty(
                    createResult.Errors.Select(e => new Error(e.Description))
                );
            }
            
            var addMemberRoleResult = await userManager.AddToRoleAsync(user, "Member");
            if (!addMemberRoleResult.Succeeded)
            {
                return Result.Fail(addMemberRoleResult.Errors.Select(e => e.Description));
            }
            
            var urlResult = await GenerateEmailConfirmationUrlAsync(user.UserName);
            if (urlResult.IsFailed)
            {
                return Result.Fail(urlResult.Errors);
            }
            
            logger.LogInformation("Confirmation Url: {UrlResultValue}", urlResult.Value);

            await emailSender.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{urlResult.Value}'>clicking here</a>."
            );
            
            return Result.Ok();
        }

        public async Task<Result> DeleteUserAsync(int id)
        {
            var user = await appDbContext.Users.FindAsync(id);
            if (user == null) return Result.Fail(UserErrors.NotFound(id));
            
            var deleteResult = await userManager.DeleteAsync(user);

            return Result.FailIfNotEmpty(
                deleteResult.Errors.Select(e => new Error(e.Description))
            );
        }
    }
}
