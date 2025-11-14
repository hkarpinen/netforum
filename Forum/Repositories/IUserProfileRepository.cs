using NETForum.Models.Entities;

namespace NETForum.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile> AddAsync(UserProfile userProfile);
    Task<UserProfile?> GetByUserIdAsync(int id, params string[] navigations);
    Task UpdateAsync(int id, Action<UserProfile> action);
    Task<bool> UserProfileExists(int userId);
}