using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Models;
using NETForum.Pages.Replies;

namespace NETForum.Services
{
    public interface IReplyService
    {
        Task<IEnumerable<Reply>> GetRepliesAsync(int postId);
        Task<int> GetTotalReplyCountAsync();
        Task<int> GetTotalReplyCountAsync(int authorId);
        Task<EntityEntry<Reply>> AddReplyAsync(int postId, Reply reply);
    }

    public class ReplyService(AppDbContext context) : IReplyService
    {
        public async Task<IEnumerable<Reply>> GetRepliesAsync(int postId)
        {
            return await context.Replies
                .Where(r => r.PostId == postId)
                .ToListAsync();
        }

        public async Task<EntityEntry<Reply>> AddReplyAsync(int postId, Reply reply)
        {
            /* var result = await context.Replies
                .AddAsync(new Reply()
                {
                    PostId = replyForm.PostId,
                    AuthorId = replyForm.AuthorId,
                    Content = replyForm.Content,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now
                }); */
            var result = await context.Replies.AddAsync(reply);
            await context.SaveChangesAsync();
            return result;
        }

        public async Task<int> GetTotalReplyCountAsync()
        {
            return await context.Replies
                .CountAsync();
        }

        public async Task<int> GetTotalReplyCountAsync(int authorId)
        {
            return await context.Replies
                .Where(r => r.AuthorId == authorId)
                .CountAsync();
        }
    }
}
