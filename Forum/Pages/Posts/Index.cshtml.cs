using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Filters;
using NETForum.Models;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    public class IndexModel(IPostService postService, IForumService forumService)
        : PageModel
    {
        [BindProperty(SupportsGet = true)] 
        public int PageNumber { get; set; } = 1;
    
        [BindProperty(SupportsGet = true)] 
        public int PageSize { get; set; } = 10;
        
        [BindProperty(SupportsGet = true)]
        public int? ForumId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? AuthorName { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool Pinned { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool Locked { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool Published { get; set; } = true;
        
        [BindProperty(SupportsGet = true)]
        public string? Title { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? ContentSearch { get; set; }
        public IEnumerable<SelectListItem> ForumSelectListItems { get; set; } = new List<SelectListItem>();
        public PagedList<PostSummaryDto> Posts { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            ForumSelectListItems = await forumService.GetForumSelectListItemsAsync();

            Posts = await postService.GetPostSummariesPagedAsync(new PostFilterOptions
            {
                ForumId = ForumId,
                AuthorName = AuthorName,
                Pinned = Pinned,
                Locked = Locked,
                Published = Published,
                Title = Title,
                Content = ContentSearch,
                SortBy = PostSortBy.CreatedAt,
                Ascending = false,
                PageSize = PageSize,
                PageNumber = PageNumber
            });
            
            return Page();
        }
    }
}
