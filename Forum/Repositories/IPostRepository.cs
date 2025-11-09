using NETForum.Models.Entities;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Repositories;

public interface IPostRepository
{
    Task<IReadOnlyCollection<Post>> GetPostsInForumAsync(int forumId, params string[] navigations);
    Task<Post> AddAsync(Post entity);
    Task<Post?> GetByIdAsync(int id, params string[] includes);
    Task<IReadOnlyCollection<Post>> GetLatestPostsAsync(int limit, params string[] navigations);
    Task<int> GetTotalPostCountByAuthorAsync(int authorId);
    Task<int> GetTotalPostCountAllTime();
    Task UpdateAsync(int id, Action<Post> updateAction);
    Task<PagedResult<Post>> GetAllPagedAsync(PagedRepositoryQueryOptions<PostFilterOptions> options);
}