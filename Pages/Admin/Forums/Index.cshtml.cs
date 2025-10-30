using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models;
using NETForum.Services;
using NETForum.Services.Criteria;

namespace NETForum.Pages.Admin.Forums
{
    [Authorize(Roles = "Admin")]
    public class IndexModel(IForumService forumService, ICategoryService categoryService)
        : PageModel
    {
        public IEnumerable<SelectListItem> CategoryListItems { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ParentForumListItems { get; set; } = new List<SelectListItem>();
        public PagedResult<Forum> Forums { get; set; } = new();

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
            CategoryListItems = await categoryService.GetCategorySelectListItems();
            ParentForumListItems = await forumService.GetSelectListItemsAsync();
            Forums = await forumService.GetForumsPagedAsync(PageNumber, PageSize, new ForumSearchCriteria()
            {
                Name = Name,
                CategoryId = CategoryId,
                ParentForumId = ParentForumId,
                Published = Published,
                SortBy = "created",
                Ascending = false
            });
            return Page();
        }
    }
}
