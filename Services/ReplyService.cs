using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Models;
using NETForum.Pages.Posts;

namespace NETForum.Services
{
    public interface IReplyService
    {
        Task<IEnumerable<PostReply>> GetRepliesAsync(int postId);
        Task<int> GetTotalReplyCountAsync();
        Task<int> GetTotalReplyCountAsync(int authorId);
        Task<EntityEntry<PostReply>> AddReplyAsync(int postId, int authorId, CreatePostReplyDto inputModel);
    }

    public class ReplyService(AppDbContext context, IMapper mapper) : IReplyService
    {
        public async Task<IEnumerable<PostReply>> GetRepliesAsync(int postId)
        {
            return await context.Replies
                .Where(r => r.PostId == postId)
                .ToListAsync();
        }

        public async Task<EntityEntry<PostReply>> AddReplyAsync(int postId, int userId, CreatePostReplyDto inputModel)
        {
            var postReply = mapper.Map<PostReply>(inputModel);
            postReply.PostId = postId;
            postReply.AuthorId = userId;
            
            var result = await context.Replies.AddAsync(postReply);
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
