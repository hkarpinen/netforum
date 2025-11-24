using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NETForum.Data;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Services.Specifications;
using FluentResults;

namespace NETForum.Services
{
    public interface IForumService
    {
        Task<IEnumerable<Forum>> GetForumsAsync();
        Task<Result<EditForumDto>> GetForumForEditAsync(int id); 
        Task<Result> UpdateForumAsync(int id, EditForumDto editForumDto);
        Task<IReadOnlyCollection<BreadcrumbItemModel>> GetForumBreadcrumbItems(int forumId);
        Task<IEnumerable<SelectListItem>> GetForumSelectListItemsAsync();
        Task<Result<Forum>> AddForumAsync(CreateForumDto createForumDto);
        Task<PagedResult<Forum>> GetForumsPagedAsync(ForumFilterOptions filterOptions);
        Task<Result<ForumPageDto>> GetForumPageDtoAsync(int forumId);
        Task<Result<ForumIndexPageDto>> GetForumIndexPageDtoAsync(int latestPostLimit = 5);
    }

    public class ForumService(IMapper mapper, AppDbContext appDbContext, IMemoryCache memoryCache) : IForumService
    {
        public async Task<Result<ForumPageDto>> GetForumPageDtoAsync(int forumId)
        {
            var dto = await appDbContext.Forums
                .Where(f => f.Id == forumId)
                .Select(f => new ForumPageDto
                {
                    Id = f.Id,
                    Title = f.Name,
                    Description = f.Description,
                    Posts = f.Posts.Select(p => new PostSummaryDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorName = p.Author.UserName,
                        AuthorAvatarUrl = p.Author.ProfileImageUrl,
                        ReplyCount = p.Replies.Count(),
                        ViewCount = p.ViewCount,
                        CreatedAt = p.CreatedAt,
                        LastReplySummary = p.Replies.OrderByDescending(p => p.CreatedAt)
                            .Select(lr => new ReplySummaryDto
                            {
                                Id = lr.Id,
                                AuthorAvatarUrl = lr.Author.ProfileImageUrl,
                                AuthorName = lr.Author.UserName,
                                CreatedAt = lr.CreatedAt
                            }).FirstOrDefault()
                    }).ToList(),
                    Subforums = f.SubForums.Select(sf => new ForumListItemDto
                    {
                        Id = sf.Id,
                        Title = sf.Name,
                        Description = sf.Description,
                        TotalPosts = sf.Posts.Count,
                        TotalReplies = sf.Posts.Count,
                        LastPostSummary = sf.Posts.OrderByDescending(p => p.CreatedAt)
                            .Select(lp => new PostSummaryDto
                            {
                                Id = lp.Id,
                                Title = lp.Title,
                                Content = lp.Content,
                                AuthorName = lp.Author.UserName,
                                AuthorAvatarUrl = lp.Author.ProfileImageUrl,
                                ReplyCount = lp.Replies.Count(),
                                ViewCount = lp.ViewCount,
                                CreatedAt = lp.CreatedAt,
                                LastReplySummary = lp.Replies.OrderByDescending(lpr => lpr.CreatedAt)
                                    .Select(lpr => new ReplySummaryDto
                                    {
                                        Id = lpr.Id,
                                        AuthorName = lpr.Author.UserName,
                                        AuthorAvatarUrl = lpr.Author.ProfileImageUrl,
                                        CreatedAt = lpr.CreatedAt
                                    })
                                    .FirstOrDefault()
                            }).FirstOrDefault(),
                    }).ToList()
                }).FirstOrDefaultAsync();

            return dto == null ? 
                Result.Fail<ForumPageDto>("Could not find forum") :
                Result.Ok(dto);

        }
        
        public async Task<Result<ForumIndexPageDto>> GetForumIndexPageDtoAsync(int latestPostLimit = 5)
        {
            var dto = new ForumIndexPageDto
            {
                RootForums = await appDbContext.Forums
                    .Where(f => f.ParentForumId == null)
                    .Select(f => new ForumListItemDto
                    {
                        Id = f.Id,
                        Title = f.Name,
                        Description = f.Description,
                        TotalPosts = f.Posts.Count,
                        TotalReplies = f.Posts.Count,
                        CategoryName = f.Category.Name,
                        LastPostSummary = f.Posts.OrderByDescending(p => p.CreatedAt)
                            .Select(lp => new PostSummaryDto
                            {
                                Id = lp.Id,
                                Title = lp.Title,
                                Content = lp.Content,
                                AuthorName = lp.Author.UserName,
                                AuthorAvatarUrl = lp.Author.ProfileImageUrl,
                                ReplyCount = lp.Replies.Count(),
                                ViewCount = lp.ViewCount,
                                CreatedAt = lp.CreatedAt,
                                LastReplySummary = lp.Replies.OrderByDescending(lpr => lpr.CreatedAt)
                                    .Select(lr => new ReplySummaryDto
                                    {
                                        Id = lr.Id,
                                        AuthorName = lr.Author.UserName,
                                        AuthorAvatarUrl = lr.Author.ProfileImageUrl,
                                        CreatedAt = lr.CreatedAt
                                    }).FirstOrDefault()
                            }).FirstOrDefault()
                    }).ToListAsync(),
                NewestUser = await appDbContext.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .FirstOrDefaultAsync(),
                LatestPosts = await appDbContext.Posts
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(latestPostLimit)
                    .Select(p => new PostTeaserDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = p.Content,
                        AuthorName = p.Author.UserName,
                        AuthorAvatarUrl = p.Author.ProfileImageUrl,
                        UpdatedAt = p.UpdatedAt
                    }).ToListAsync(),
                Stats = await memoryCache.GetOrCreateAsync("HomeStats", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    return new HomeStatsDto
                    {
                        TotalPosts = await appDbContext.Posts.CountAsync(),
                        TotalMembers = await appDbContext.Users.CountAsync(),
                        TotalReplies = await appDbContext.Posts.CountAsync()
                    };
                })
            };
            return Result.Ok(dto);
        }
        
        public async Task<IEnumerable<Forum>> GetForumsAsync()
        {
            return await appDbContext.Forums.ToListAsync();
        }

        public async Task<Result<EditForumDto>> GetForumForEditAsync(int id)
        {
            var forum = await appDbContext.Forums.FindAsync(id);
            if (forum == null)
            {
                return Result.Fail<EditForumDto>("Could not find forum");
            }
            var editForumDto = mapper.Map<EditForumDto>(forum);
            return Result.Ok(editForumDto);
        }
        
        public async Task<Result> UpdateForumAsync(int forumId, EditForumDto editForumDto)
        {
            try
            {
                var forum = await appDbContext.Forums.FindAsync(forumId);
                if (forum == null)
                {
                    return Result.Fail("Could not find forum");
                }
                
                // Map changes to forum
                mapper.Map(editForumDto, forum);
                await appDbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (UniqueConstraintException exception)
            {
                var constraintNameShort = exception.ConstraintName.Split("_").Last();
                return Result.Fail($"Forum {constraintNameShort} already exists.");
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetForumSelectListItemsAsync()
        {
            return await appDbContext.Forums
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name,
                }).ToListAsync();
        }
        
        public async Task<Result<Forum>> AddForumAsync(CreateForumDto createForumDto) 
        {
            try
            {
                var forum = mapper.Map<Forum>(createForumDto);
                var result = await appDbContext.Forums.AddAsync(forum);
                await appDbContext.SaveChangesAsync();
                return Result.Ok(result.Entity);
            }
            catch (UniqueConstraintException exception)
            {
                var constraintNameShort = exception.ConstraintName.Split("_").Last();
                return Result.Fail<Forum>($"Forum {constraintNameShort} already exists.");
            }
        }
        
        public async Task<IReadOnlyCollection<BreadcrumbItemModel>> GetForumBreadcrumbItems(int forumId)
        {
            // Recursively find parent forums
            Stack<Forum> stack = new();
            var forum = await appDbContext.Forums.FindAsync(forumId);
            
            while (forum != null)
            {
                stack.Push(forum);
                if (forum.ParentForumId.HasValue)
                {
                    forum = await appDbContext.Forums.FindAsync(forum.ParentForumId.Value);
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
            }).ToList();
        }
        
        public async Task<PagedResult<Forum>> GetForumsPagedAsync(ForumFilterOptions filterOptions) {
            var forumSearchSpec = new ForumSearchSpec(filterOptions);
            
            var totalForums = await appDbContext.Forums.CountAsync();
            var forums = await appDbContext.Forums
                .WithSpecification(forumSearchSpec)
                .ToListAsync();
            var pagedResult = new PagedResult<Forum>
            {
                TotalCount = totalForums,
                Items = forums,
                PageSize = filterOptions.PageSize,
                PageNumber = filterOptions.PageNumber
            };
            return pagedResult;
        }
    }
}
