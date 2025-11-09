using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Pages.Shared.Components.ForumList;
using NETForum.Repositories;
using NETForum.Repositories.Filters;

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
        Task<Forum> CreateForumAsync(CreateForumDto createForumDto);
        Task<PagedResult<Forum>> GetForumsPagedAsync(
            int pageNumber, 
            int pageSize, 
            ForumFilterOptions filterOptions,
            string? sortBy,
            bool ascending
        );
    }

    public class ForumService(IForumRepository forumRepository, IMapper mapper) : IForumService
    {
        public async Task<IEnumerable<Forum>> GetForumsAsync()
        {
            var options = new RepositoryQueryOptions<ForumFilterOptions>();
            return await forumRepository.GetAllAsync(options);
        }

        public async Task<IEnumerable<ForumListItemModel>> GetRootForumListItemsAsync()
        {
            var rootForumsNavigations = new[]
            {
                "Category",
                "Posts.Replies"
            };
            var rootForums = await forumRepository.GetAllRootForumsAsync(rootForumsNavigations);
            return rootForums.Select(rf => new ForumListItemModel
            {
                Forum = rf,
                PostCount = rf.Posts.Count,
                RepliesCount = rf.Posts.Sum(p => p.Replies.Count()),
                LastPost = rf.Posts.OrderByDescending(p => p.CreatedAt).FirstOrDefault(),
                
                // TODO: Created at is not the right date to use here?
                LastUpdated = rf.Posts.Count != 0 ? rf.Posts.Max(p => p.UpdatedAt) : rf.CreatedAt
            });

        }

        public async Task<Forum?> GetForumByIdAsync(int id)
        {
            return await forumRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateForum(EditForumDto editForumDto)
        {
            try
            {
                await forumRepository.UpdateAsync(editForumDto.Id, trackedForum =>
                {
                    mapper.Map(editForumDto, trackedForum);
                });
            }
            catch (KeyNotFoundException exception)
            {
                // TODO: Need to setup logging for this. Log to external file or service maybe?
                return false;
            }
            
            // Update succeeded.
            return true;

        }

        public async Task<IEnumerable<SelectListItem>> GetSelectListItemsAsync()
        {
            var forumFilterOptions = new RepositoryQueryOptions<ForumFilterOptions>();
            var allForums = await forumRepository.GetAllAsync(forumFilterOptions);
            return allForums.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name
            });
        }

        // TODO: Rename method to AddForumAsync() for consistency.
        public async Task<Forum> CreateForumAsync(CreateForumDto createForumDto) 
        {
            var forum = mapper.Map<Forum>(createForumDto);
            var result = await forumRepository.AddAsync(forum);
            return result;
        }

        // TODO: Rename method to GetChildForumListItemsWithPostsAndReplies()
        public async Task<IEnumerable<ForumListItemModel>> GetForumListItemsAsync(int parentForumId)
        {
            var forums = await forumRepository.GetForumPostsWithReplies(parentForumId);
            return forums.Select(f => new ForumListItemModel
            {
                Forum = f,
                PostCount = f.Posts.Count,
                RepliesCount = f.Posts.Sum(p => p.Replies.Count()),
                LastPost = f.Posts.OrderByDescending(p => p.CreatedAt).FirstOrDefault(),
                LastUpdated = f.Posts.Count != 0 ? f.Posts.Max(p => p.UpdatedAt) : f.CreatedAt
            });
        }
        
        public async Task<IEnumerable<BreadcrumbItemModel>> GetForumBreadcrumbItems(int forumId)
        {
            // Recursively find parent forums
            Stack<Forum> stack = new();
            var forum = await forumRepository.GetByIdAsync(forumId);
            while (forum != null)
            {
                stack.Push(forum);
                if (forum.ParentForumId.HasValue)
                {
                    forum = await forumRepository.GetByIdAsync(forum.ParentForumId.Value);
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

        // TODO: Rename method to GetForumsPagedWithCategoryAndParentForumAsync()
        // TODO: Might be better to create a repository method that does this, then manually building query options.
        public async Task<PagedResult<Forum>> GetForumsPagedAsync(
            int pageNumber, 
            int pageSize, 
            ForumFilterOptions filterOptions,
            string? sortBy,
            bool ascending
        ) {
            var repositoryPagedQueryOptions = new PagedRepositoryQueryOptions<ForumFilterOptions>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = filterOptions,
                Navigations = ["Category", "ParentForum"],
                SortBy = sortBy,
                Ascending = ascending
            };

            return await forumRepository.GetAllPagedAsync(repositoryPagedQueryOptions);
        }
    }
}
