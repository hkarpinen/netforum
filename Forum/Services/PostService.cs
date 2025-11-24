using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services.Specifications;
using FluentResults;

namespace NETForum.Services
{
    public interface IPostService
    {
        Task<Result<EditPostDto>> GetPostForEditAsync(int id);
        Task<Result<Post>> AddPostAsync(string username, int forumId, CreatePostDto createPostDto);
        Task<Result> UpdatePostAsync(int id, EditPostDto editPostDto);
        Task<PagedResult<PostSummaryDto>> GetPostsPagedAsync(PostFilterOptions postFilterOptions);
        Task<Result<PostPageDto>> GetPostPageDto(int postId, string viewerUsername);
    }

    public class PostService(IMapper mapper, AppDbContext appDbContext) : IPostService
    {
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
                Result.Fail<PostPageDto>("Could not find post") :
                Result.Ok(dto);
        }
        
        public async Task<Result> UpdatePostAsync(int id, EditPostDto editPostDto)
        {
            try
            {
                var post = await appDbContext.Posts.FindAsync(id);
                mapper.Map(editPostDto, post);
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
                var author = await appDbContext.Users.FindAsync(username);
                if (author == null)
                {
                    return Result.Fail<Post>("Author not found");
                }

                // Map the DTO to a Post() instance.
                var post = mapper.Map<Post>(createPostDto);
                post.AuthorId = author.Id;
                post.ForumId = forumId;
                var result = await appDbContext.Posts.AddAsync(post);
                await appDbContext.SaveChangesAsync();
                return Result.Ok(result.Entity);
            }
            catch (UniqueConstraintException exception)
            {
                var shortConstraintName = exception.ConstraintName.Split("_").Last();
                return Result.Fail<Post>($"{shortConstraintName} is already taken.");
            }
        }

        public async Task<Result<EditPostDto>> GetPostForEditAsync(int id)
        {
            var post = await appDbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return Result.Fail<EditPostDto>("Post not found");
            }
            var editPostDto = mapper.Map<EditPostDto>(post);
            return Result.Ok(editPostDto);
        } 
        
        public async Task<PagedResult<PostSummaryDto>> GetPostsPagedAsync(PostFilterOptions postFilterOptions) {
            
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
