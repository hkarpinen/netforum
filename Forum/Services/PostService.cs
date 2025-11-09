using AutoMapper;
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
        Task<Post?> GetPostWithAuthorAndRepliesAsync(int id);
        Task<Post> AddPostAsync(string username, int forumId, CreatePostDto createPostDto);
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
        
        public async Task<Post> AddPostAsync(string username, int forumId, CreatePostDto createPostDto)
        {
                var author = await userService.GetUserAsync(username);
                if(author == null) throw new Exception("User not found");
                
                var post = mapper.Map<Post>(createPostDto);
                post.AuthorId = author.Id;
                post.ForumId = forumId;
                
                var result = await postRepository.AddAsync(post);
                return result;
        }
        
        public async Task<Post?> GetPostWithAuthorAndRepliesAsync(int id)
        {
            var navigations = new[]
            {
                "Author",
                "Replies"
            };
            return await postRepository.GetByIdAsync(id, navigations);
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
