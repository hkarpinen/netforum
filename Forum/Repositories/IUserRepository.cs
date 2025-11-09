using NETForum.Models.Entities;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetAllAsync(RepositoryQueryOptions<UserFilterOptions> options);
    Task<PagedResult<User>> GetAllPagedAsync(PagedRepositoryQueryOptions<UserFilterOptions> options);
    Task<User?> GetByIdAsync(int id, params string[] includes);
    Task<User?> GetByUsernameAsync(string username, params string[] includes);
    Task UpdateAsync(int id, Action<User> updateAction);
    Task DeleteByIdAsync(int id);
    Task<User?> GetNewestUserAsync(params string[] includes);
    Task<int> CountTotalUsersAsync();
    Task<DateTime> GetUserJoinedDateAsync(int id);
}