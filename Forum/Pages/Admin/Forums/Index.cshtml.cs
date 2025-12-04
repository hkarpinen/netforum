using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Filters;
using NETForum.Models;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Admin.Forums
{
    [Authorize(Roles = "Admin")]
    public class IndexModel(IForumService forumService, ICategoryService categoryService)
        : PageModel
    {
        public IEnumerable<SelectListItem> CategoryListItems { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ParentForumListItems { get; set; } = new List<SelectListItem>();
        public PagedList<Forum> Forums { get; set; } = new();

        [BindProperty(SupportsGet = true)] 
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)] 
        public int PageSize { get; set; } = 10;
        
        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? ParentForumId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool? Published { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CategoryListItems = await categoryService.GetCategorySelectListItemsAsync();
            ParentForumListItems = await forumService.GetForumSelectListItemsAsync();
            Forums = await forumService.GetForumsPagedAsync(new ForumFilterOptions
            {
                Name = Name,
                CategoryId = CategoryId,
                ParentForumId = ParentForumId,
                Published = Published,
                PageNumber = PageNumber,
                PageSize = PageSize,
                SortBy = ForumSearchSortBy.CreatedAt,
                Ascending = false
            });
            return Page();
        }
    }
}
