using NETForum.Models.Entities;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Repositories;

public interface IForumRepository
{
    Task<IReadOnlyCollection<Forum>> GetAllAsync(RepositoryQueryOptions<ForumFilterOptions> options);
    Task<PagedResult<Forum>> GetAllPagedAsync(PagedRepositoryQueryOptions<ForumFilterOptions> options);
    Task<IReadOnlyCollection<Forum>> GetAllRootForumsAsync(params string[] includes);
    Task<IReadOnlyCollection<Forum>> GetChildForumsAsync(int forumId, params string[] includes);
    Task<Forum?> GetByIdAsync(int id, params string[] includes);
    Task<Forum> AddAsync(Forum entity);
    Task UpdateAsync(int id, Action<Forum> updateAction);
}
