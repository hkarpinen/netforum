using AutoMapper;
using NETForum.Models.Components;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;
using NETForum.Repositories.Filters;

namespace NETForum.Services
{
    public interface IPostService
    {
        Task<IReadOnlyCollection<Post>> GetPostsAsync(int forumId);
        Task IncrementViewCountAsync(int postId);
        Task<IEnumerable<PostListItem>> GetPostListItemsAsync(int forumId);
        Task<int> GetTotalPostCountAsync();
        Task<int> GetTotalPostCountAsync(int authorid);
        Task<Post?> GetPostWithAuthorAndRepliesAsync(int id);
        Task<Post> CreatePostAsync(string username, int forumId, CreatePostDto createPostDto);
        Task<IEnumerable<Post>> GetLatestPostsAsync(int limit);
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
        
        // TODO: Rename method to AddForumAsync() for consistency
        public async Task<Post> CreatePostAsync(string username, int forumId, CreatePostDto createPostDto)
        {
                var author = userService.GetUserAsync(username);
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

        // TODO: Rename to GetLatestPostsWithAuthorAsync
        public async Task<IEnumerable<Post>> GetLatestPostsAsync(int limit)
        {
            var navigations = new[] {  "Author" };
            return await postRepository.GetLatestPostsAsync(limit, navigations);
        }

        public async Task<IEnumerable<PostListItem>> GetPostListItemsAsync(int forumId)
        {
            var navigations = new[] {  "Author", "Replies.Author" };
            var posts = await postRepository.GetPostsInForumAsync(forumId, navigations);
            return posts.Select(p => new PostListItem()
            {
                Id = p.Id,
                // TODO: Author ID is stored on Author, doesn't need to be here twice.
                ForumId = p.ForumId,
                AuthorId = p.AuthorId,
                Author = p.Author,
                IsPinned = p.IsPinned,
                IsLocked = p.IsLocked,
                Published = p.Published,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                ViewCount = p.ViewCount,
                LastReply = p.Replies.OrderByDescending(r => r.CreatedAt).FirstOrDefault()
            });
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            try
            {
                await postRepository.UpdateAsync(postId, trackedPost => { trackedPost.ViewCount++; });
            }
            catch (KeyNotFoundException exception)
            {
                // TODO: Need to setup logging for this. 
            }
        }

        public async Task<int> GetTotalPostCountAsync()
        {
            return await postRepository.GetTotalPostCountAllTime();
        }

        // TODO: Rename to GetTotalPostCountByAuthorAsync()
        public async Task<int> GetTotalPostCountAsync(int authorid)
        {
            return await postRepository.GetTotalPostCountByAuthorAsync(authorid);
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
