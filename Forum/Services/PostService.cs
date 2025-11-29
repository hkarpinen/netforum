using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services.Specifications;
using FluentResults;
using NETForum.Errors;

namespace NETForum.Services
{
    public interface IPostService
    {
        Task<Result<EditPostDto>> GetPostForEditAsync(int id);
        Task<Result<Post>> AddPostAsync(string username, int forumId, CreatePostDto createPostDto);
        Task<Result> UpdatePostAsync(int id, EditPostDto editPostDto);
        Task<PagedResult<PostSummaryDto>> GetPostSummariesPagedAsync(PostFilterOptions postFilterOptions);
        Task<Result<PostPageDto>> GetPostPageDto(int postId, string viewerUsername);
        Task<Result<Post>> GetPostAsync(int id);
    }

    public class PostService(AppDbContext appDbContext) : IPostService
    {
        public async Task<Result<Post>> GetPostAsync(int id)
        {
            var post = await appDbContext.Posts.Where(p => p.Id == id).FirstOrDefaultAsync();
            return post == null ? 
                Result.Fail<Post>(PostErrors.NotFound(id)) : 
                Result.Ok(post);
        }
        
        public async Task<Result<PostPageDto>> GetPostPageDto(int postId, string? viewerUsername)
        {
            var dto = await appDbContext.Posts
                .Where(p => p.Id == postId)
                .Select(p => new PostPageDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.UserName,
                    AuthorAvatarImageUrl = p.Author.ProfileImageUrl,
                    Replies = p.Replies.Select(r => new ReplyViewModel
                    {
                        Id = r.Id,
                        AuthorName = r.Author.UserName,
                        AuthorAvatarImageUrl = r.Author.ProfileImageUrl,
                        PostTitle = r.Post.Title,
                        Content = r.Content,
                        CreatedAt = r.CreatedAt
                    }).ToList(),
                    AuthorStatsSummary = new AuthorStatsSummary
                    {
                        TotalPostCount = p.Author.Posts.Count(),
                        TotalReplyCount = p.Author.Replies.Count(),
                        JoinedOn = p.Author.CreatedAt
                    },
                    CurrentUserIsAuthor = viewerUsername != null && viewerUsername == p.Author.UserName
                }).FirstOrDefaultAsync();
            
            return dto == null ? 
                Result.Fail<PostPageDto>(PostErrors.NotFound(postId)) :
                Result.Ok(dto);
        }
        
        public async Task<Result> UpdatePostAsync(int id, EditPostDto editPostDto)
        {
            var post = await appDbContext.Posts.FindAsync(id);
            
            // Handle post not found
            if (post == null)
            {
                return Result.Fail(PostErrors.NotFound(id));
            }
            
            // If author is not found
            if (!await appDbContext.Users.AnyAsync(u => u.Id == editPostDto.AuthorId))
            {
                return Result.Fail(PostErrors.AuthorNotFound(editPostDto.AuthorId));
            }
            
            // If forum doesn't exist
            if (!await appDbContext.Forums.AnyAsync(f => f.Id == editPostDto.ForumId))
            {
                return Result.Fail(PostErrors.NonExistentForumId(editPostDto.ForumId));
            }
            
            try
            {
                // Map Edit DTO to Post
                post.Title = editPostDto.Title;
                post.Content = editPostDto.Content;
                post.ForumId = editPostDto.ForumId;
                post.AuthorId = editPostDto.AuthorId;
                post.IsPinned = editPostDto.IsPinned;
                post.IsLocked = editPostDto.IsLocked;
                post.Published = editPostDto.Published;
                post.UpdatedAt = DateTime.UtcNow;
                
                await appDbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (DbUpdateException exception)
            {
                return Result.Fail("Could not update post");
            }
        }
        
        public async Task<Result<Post>> AddPostAsync(string username, int forumId, CreatePostDto createPostDto)
        {
            try
            {
                
                var author = await appDbContext.Users.Where(u => u.UserName == username)
                    .FirstOrDefaultAsync();
                
                // If author does not exist.
                if (author == null)
                {
                    return Result.Fail<Post>(PostErrors.AuthorNotFound(username));
                }
                
                // If forum does not exist
                if (!await appDbContext.Forums.AnyAsync(f => f.Id == forumId))
                {
                    return Result.Fail<Post>(PostErrors.NonExistentForumId(forumId));
                }

                // Map Create DTO to Post
                var post = new Post
                {
                    Title = createPostDto.Title,
                    Content = createPostDto.Content,
                    ForumId = forumId,
                    IsPinned = createPostDto.IsPinned,
                    IsLocked = createPostDto.IsLocked,
                    Published = createPostDto.Published,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AuthorId = author.Id
                };
                
                var result = await appDbContext.Posts.AddAsync(post);
                await appDbContext.SaveChangesAsync();
                return Result.Ok(result.Entity);
            }
            catch (Exception exception)
            {
                return Result.Fail<Post>(exception.Message);
            }
        }

        public async Task<Result<EditPostDto>> GetPostForEditAsync(int id)
        {
            var post = await appDbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return Result.Fail<EditPostDto>(PostErrors.NotFound(id));
            }
            
            // Map Post to Edit DTO
            var editPostDto = new EditPostDto
            {
                Title = post.Title,
                Content = post.Content,
                ForumId = post.ForumId,
                AuthorId = post.AuthorId,
                IsPinned = post.IsPinned,
                IsLocked = post.IsLocked,
                Published = post.Published
            };
            
            return Result.Ok(editPostDto);
        } 
        
        public async Task<PagedResult<PostSummaryDto>> GetPostSummariesPagedAsync(PostFilterOptions postFilterOptions) {
            
            var postSearchSpec = new PostSearchSpec(postFilterOptions);
            var totalPosts = await appDbContext.Posts.CountAsync();
            var posts = await appDbContext.Posts
                .WithSpecification(postSearchSpec)
                .Select(p => new PostSummaryDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    AuthorName = p.Author.UserName,
                    AuthorAvatarUrl = p.Author.ProfileImageUrl,
                    ReplyCount = p.Replies.Count(),
                    ViewCount = p.ViewCount,
                    LastReplySummary = p.Replies.OrderByDescending(r => r.CreatedAt)
                        .Select(r => new ReplySummaryDto
                        {
                            Id = r.Id,
                            AuthorName = r.Author.UserName,
                            AuthorAvatarUrl = r.Author.ProfileImageUrl,
                            CreatedAt = r.CreatedAt
                        }).FirstOrDefault(),
                })
                .ToListAsync();
            var pagedResult = new PagedResult<PostSummaryDto>
            {
                TotalCount = totalPosts,
                PageSize = postFilterOptions.PageSize,
                PageNumber = postFilterOptions.PageNumber,
                Items = posts
            };
            return pagedResult;
        }
    }
}
