using AutoMapper;
using EntityFramework.Exceptions.Common;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;
using NETForum.Repositories.Filters;

namespace NETForum.Services
{
    public interface IPostService
    {
        Task<IReadOnlyCollection<Post>> GetPostsAsync(int forumId);
        Task<int> GetTotalPostCountAsync();
        Task<int> GetTotalPostCountByAuthorAsync(int authorId);
        Task<Result<Post>> GetPostWithAuthorAndRepliesAsync(int id);
        Task<Result<Post>> AddPostAsync(string username, int forumId, CreatePostDto createPostDto);
        Task<IReadOnlyCollection<Post>> GetLatestPostsWithAuthorAsync(int limit);
        Task<PagedResult<Post>> GetPostsPagedAsync(
            int pageNumber, 
            int pageSize, 
            PostFilterOptions postFilterOptions,
            string? sortBy,
            bool ascending
        );
    }

    public class PostService(IMapper mapper, IUserService userService, IPostRepository postRepository) : IPostService
    {
        public async Task<IReadOnlyCollection<Post>> GetPostsAsync(int forumId)
        {
            var navigations = new[]
            {
                "Author",
                "Replies.Author"
            };
            return await postRepository.GetPostsInForumAsync(forumId, navigations);
        }
        
        public async Task<Result<Post>> AddPostAsync(string username, int forumId, CreatePostDto createPostDto)
        {
            try
            {
                var authorLookupResult = await userService.GetByUsernameAsync(username);
                if (authorLookupResult.IsFailure)
                {
                    return Result<Post>.Failure(authorLookupResult.Error);
                }

                // Map the DTO to a Post() instance.
                var post = mapper.Map<Post>(createPostDto);
                post.AuthorId = authorLookupResult.Value.Id;
                post.ForumId = forumId;
                var result = await postRepository.AddAsync(post);
                return Result<Post>.Success(result);
            }
            catch (UniqueConstraintException exception)
            {
                var shortConstraintName = exception.ConstraintName.Split("_").Last();
                return Result<Post>.Failure(new Error("Post.UniqueConstraintViolation", $"{shortConstraintName} is already taken."));
            }
        }
        
        public async Task<Result<Post>> GetPostWithAuthorAndRepliesAsync(int id)
        {
            var navigations = new[]
            {
                "Author",
                "Replies"
            };
            var post = await postRepository.GetByIdAsync(id, navigations);
            return post == null ? 
                Result<Post>.Failure(new Error("Post.NotFound", $"Post with {id} not found.")) : 
                Result<Post>.Success(post);
        }
        
        public async Task<IReadOnlyCollection<Post>> GetLatestPostsWithAuthorAsync(int limit)
        {
            var navigations = new[] {  "Author" };
            return await postRepository.GetLatestPostsAsync(limit, navigations);
        }

        public async Task<int> GetTotalPostCountAsync()
        {
            return await postRepository.GetTotalPostCountAllTime();
        }
        
        public async Task<int> GetTotalPostCountByAuthorAsync(int authorId)
        {
            return await postRepository.GetTotalPostCountByAuthorAsync(authorId);
        }
        
        public async Task<PagedResult<Post>> GetPostsPagedAsync(
            int pageNumber, 
            int pageSize,
            PostFilterOptions postFilterOptions,
            string? sortBy,
            bool ascending
        )
        {
            var repositoryPagedQueryOptions = new PagedRepositoryQueryOptions<PostFilterOptions>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = postFilterOptions,
                Navigations = ["Author", "Replies"],
                SortBy = sortBy,
                Ascending = ascending
            };
            return await postRepository.GetAllPagedAsync(repositoryPagedQueryOptions);
        }
    }
}
