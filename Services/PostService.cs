using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Models.Components;
using NETForum.Pages.Posts;
using NETForum.Services.Criteria;

namespace NETForum.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetPostsAsync(int forumId);
        Task IncrementViewCountAsync(int postId);
        Task<IEnumerable<PostListItem>> GetPostListItemsAsync(int forumId);
        Task<int> GetTotalPostCountAsync();
        Task<int> GetTotalPostCountAsync(int authorid);
        Task<Post?> GetPostAsync(int id);
        Task<EntityEntry<Post>> CreatePostAsync(Post post);
        Task<IEnumerable<Post>> GetLatestPostsAsync(int limit);
        Task<PagedResult<Post>> GetPostsPagedAsync(int pageNumber, int pageSize, PostSearchCriteria postSearchCriteria);
    }

    public class PostService(AppDbContext context) : IPostService
    {
        public async Task<IEnumerable<Post>> GetPostsAsync(int forumId)
        {
            return await context.Posts
                .Where(p => p.ForumId == forumId)
                .Include(p => p.Author)
                .Include(p => p.Replies)
                .ThenInclude(r => r.Author)
                .ToListAsync();
        }
        
        public async Task<EntityEntry<Post>> CreatePostAsync(Post post)
        {
                var result = await context.Posts.AddAsync(post);
                await context.SaveChangesAsync();
                return result;
        }

        public async Task<Post?> GetPostAsync(int id)
        {
            return await context.Posts
                .Where(p => p.Id == id)
                .Include(p => p.Author)
                .Include(p => p.Replies)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Post>> GetLatestPostsAsync(int limit)
        {
            return await context.Posts
                .Include(p => p.Author)
                .OrderByDescending(p => p.UpdatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<PostListItem>> GetPostListItemsAsync(int forumId)
        {
            return await context.Posts
                .Where(p => p.ForumId == forumId)
                .Include(p => p.Author)
                .Include(p => p.Replies)
                .ThenInclude(r => r.Author)
                .Select(p => new PostListItem
                {
                    Id = p.Id,
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
                    LastReply = p.Replies
                        .OrderByDescending(r => r.CreatedAt)
                        .FirstOrDefault() 
                })
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            Post? post = null;
            post = await context.Posts
                .Where(p => p.Id == postId)
                .FirstOrDefaultAsync();
            if(post != null)
            {
                post.ViewCount = post.ViewCount + 1;
                context.Posts.Update(post);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task<int> GetTotalPostCountAsync()
        {
            return await context.Posts
                .CountAsync();
        }

        public async Task<int> GetTotalPostCountAsync(int authorid)
        {
            return await context.Posts
                .Where(p => p.AuthorId == authorid)
                .CountAsync();
        }
        
        public async Task<PagedResult<Post>> GetPostsPagedAsync(
            int pageNumber, 
            int pageSize,
            PostSearchCriteria postSearchCriteria
        )
        {
            var query = context.Posts
                .WhereForum(postSearchCriteria.ForumId)
                .WhereAuthor(postSearchCriteria.AuthorId)
                .WherePinned(postSearchCriteria.Pinned)
                .WhereLocked(postSearchCriteria.Locked)
                .WherePublished(postSearchCriteria.Published)
                .WhereTitle(postSearchCriteria.Title)
                .WhereContent(postSearchCriteria.Content)
                .OrderByField(postSearchCriteria.SortBy, postSearchCriteria.Ascending);
            
            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Author)
                .Include(p => p.Replies)
                .ToListAsync();

            return new PagedResult<Post>
            {
                Items = posts,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
