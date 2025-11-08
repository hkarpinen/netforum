using NETForum.Models.Entities;

namespace NETForum.Repositories;

public interface IReplyRepository
{
    Task<IReadOnlyCollection<Reply>> GetRepliesByPostAsync(int postId, params string[] navigations);
    Task<int> GetTotalReplyCountAsync();
    Task<int> GetTotalReplyCountByAuthorAsync(int authorId);
    Task UpdateAsync(int id, Action<Reply> action);
    Task<Reply?> GetByIdAsync(int id, params string[] navigations);
    Task<Reply> AddAsync(Reply entity);
}