using AutoMapper;
using NETForum.Data;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;

namespace NETForum.Services
{
    public interface IReplyService
    {
        Task<IEnumerable<Reply>> GetRepliesAsync(int postId);
        Task<int> GetTotalReplyCountAsync();
        Task<int> GetTotalReplyCountAsync(int authorId);
        Task<Result<Reply>> AddReplyAsync(int postId, int authorId, CreatePostReplyDto inputModel);
    }

    public class ReplyService(AppDbContext context, IMapper mapper, IReplyRepository replyRepository) : IReplyService
    {
        public async Task<IEnumerable<Reply>> GetRepliesAsync(int postId)
        {
            return await replyRepository.GetRepliesByPostAsync(postId);
        }

        public async Task<Result<Reply>> AddReplyAsync(int postId, int userId, CreatePostReplyDto inputModel)
        {
            try
            {
                var postReply = mapper.Map<Reply>(inputModel);
                postReply.PostId = postId;
                postReply.AuthorId = userId;
            
                var result = await replyRepository.AddAsync(postReply);
                await context.SaveChangesAsync();
                return Result<Reply>.Success(result);
            }
            catch (Exception e)
            {
                return Result<Reply>.Failure(new Error("Reply.UnknownError", e.Message));
            }
        }

        public async Task<int> GetTotalReplyCountAsync()
        {
            return await replyRepository.GetTotalReplyCountAsync();
        }

        public async Task<int> GetTotalReplyCountAsync(int authorId)
        {
            return await replyRepository.GetTotalReplyCountByAuthorAsync(authorId);
        }
    }
}
