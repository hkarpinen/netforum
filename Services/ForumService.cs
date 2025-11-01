using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Pages.Forums;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Pages.Shared.Components.ForumList;
using NETForum.Services.Criteria;

namespace NETForum.Services
{
    public interface IForumService
    {
        Task<IEnumerable<Forum>> GetForumsAsync();
        Task<IEnumerable<ForumListItemModel>> GetRootForumListItemsAsync();
        Task<Forum?> GetForumByIdAsync(int id);
        Task<bool> UpdateForum(EditForumDto editForumDto);
        Task<IEnumerable<ForumListItemModel>> GetForumListItemsAsync(int parentForumId);
        Task<IEnumerable<BreadcrumbItemModel>> GetForumBreadcrumbItems(int forumId);
        Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync();
        Task<EntityEntry<Forum>> CreateForumAsync(CreateForumDto createForumDto);
        Task<PagedResult<Forum>> GetForumsPagedAsync(int pageNumber, int pageSize, ForumSearchCriteria searchCriteria);
    }

    public class ForumService(AppDbContext context, IMapper mapper) : IForumService
    {
        public async Task<IEnumerable<Forum>> GetForumsAsync()
        {
            return await context.Forums.ToListAsync();
        }

        public async Task<IEnumerable<ForumListItemModel>> GetRootForumListItemsAsync()
        {
            return await context.Forums
                .Where(f => f.ParentForumId == null)
                .Include(f => f.Category)
                .Include(f => f.Posts)
                .ThenInclude(p => p.Replies)
                .Select(f => new ForumListItemModel
                {
                    Forum = f,
                    PostCount = f.Posts.Count(),
                    RepliesCount = f.Posts.Sum(p => p.Replies.Count()),
                    LastPost = f.Posts.OrderByDescending(p => p.CreatedAt).FirstOrDefault(),
                    LastUpdated = f.Posts.Any()
                        ? f.Posts.Max(p => p.CreatedAt)
                        : f.CreatedAt
                }).ToListAsync();
        }

        public async Task<Forum?> GetForumByIdAsync(int id)
        {
            return await context.Forums.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<bool> UpdateForum(EditForumDto editForumDto)
        {
            var forum = await context.Forums.FindAsync(editForumDto.Id);
            if (forum == null) return false;
            
            mapper.Map(editForumDto, forum);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync()
        {
            return await context.Forums
                .Select(f => new SelectListItem()
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                }).ToListAsync();
        }

        public async Task<EntityEntry<Forum>> CreateForumAsync(CreateForumDto createForumDto) 
        {
            var forum = mapper.Map<Forum>(createForumDto);
            var result = context.Forums.Add(forum);
            await context.SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<ForumListItemModel>> GetForumListItemsAsync(int parentForumId)
        {
            return await context.Forums
                .Where(f => f.ParentForumId == parentForumId)
                .Include(f => f.Posts)
                .ThenInclude(p => p.Replies)
                .Select(f => new ForumListItemModel
                {
                    Forum = f,
                    PostCount = f.Posts.Count(),
                    RepliesCount = f.Posts.Sum(p => p.Replies.Count()),
                    LastPost = f.Posts.OrderByDescending(p => p.CreatedAt).FirstOrDefault(),
                    LastUpdated = f.Posts.Any()
                        ? f.Posts.Max(p => p.CreatedAt)
                        : f.CreatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<ForumListItemModel>> GetForumListItemsAsync()
        {
            return await context.Forums
                .Include(f => f.Posts)
                .ThenInclude(p => p.Replies)
                .Select(f => new ForumListItemModel
                {
                    Forum = f,
                    PostCount = f.Posts.Count(),
                    RepliesCount = f.Posts.Sum(p => p.Replies.Count()),
                    LastPost = f.Posts.OrderByDescending(p => p.CreatedAt).FirstOrDefault(),
                    LastUpdated = f.Posts.Any()
                        ? f.Posts.Max(p => p.CreatedAt)
                        : f.CreatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<BreadcrumbItemModel>> GetForumBreadcrumbItems(int forumId)
        {
            // Recursively find parent forums
            Stack<Forum> stack = new();
            var forum = await context.Forums.FirstOrDefaultAsync(f => f.Id == forumId);

            while (forum != null)
            {
                stack.Push(forum);
                if (forum.ParentForumId != null)
                {
                    forum = await context.Forums.FirstOrDefaultAsync(f => f.Id == forum.ParentForumId);
                }
                else
                {
                    forum = null;
                }
            }

            // Map the resulting stack to Breadcrumb Items
            return stack.Select((f, index) => new BreadcrumbItemModel
            {
                Text = f.Name,
                Url = $"/Forums/{f.Id}",
                Active = index == stack.Count - 1
            });
        }

        public async Task<PagedResult<Forum>> GetForumsPagedAsync(int pageNumber, int pageSize, ForumSearchCriteria searchCriteria)
        {
            var query = context.Forums
                .WhereName(searchCriteria.Name)
                .WhereCategory(searchCriteria.CategoryId)
                .WhereParentForum(searchCriteria.ParentForumId)
                .WherePublished(searchCriteria.Published)
                .OrderByField(searchCriteria.SortBy, searchCriteria.Ascending);

            var totalCount = await query.CountAsync();
            var forums = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(f => f.Category)
                .Include(f => f.ParentForum)
                .ToListAsync();

            return new PagedResult<Forum>
            {
                Items = forums,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
