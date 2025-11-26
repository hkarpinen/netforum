using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using FluentResults;

namespace NETForum.Services
{
    public interface IReplyService
    {
        Task<Result<Reply>> AddReplyAsync(int postId, int authorId, CreatePostReplyDto inputModel);
    }

    public class ReplyService(AppDbContext appDbContext) : IReplyService
    {
        public async Task<Result<Reply>> AddReplyAsync(int postId, int userId, CreatePostReplyDto inputModel)
        {
            try
            {
                // Map Create DTO to Reply
                var postReply = new Reply
                {
                    Content = inputModel.Content,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    PostId = postId,
                    AuthorId = userId
                };
            
                var result = await appDbContext.Replies.AddAsync(postReply);
                await appDbContext.SaveChangesAsync();
                return Result.Ok(result.Entity);
            }
            catch (DbUpdateException e)
            {
                return Result.Fail<Reply>("Could not add reply.");
            }
        }
    }
}
