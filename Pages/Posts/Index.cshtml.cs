using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.Entities;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    public class IndexModel(IPostService postService, IForumService forumService, IUserService userService)
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
        public PagedResult<Post> Posts { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            ForumSelectListItems = await forumService.GetSelectListItemsAsync();

            int? authorId = null;
            
            if (AuthorName != null)
            {
                var author = await userService.GetUserAsync(AuthorName);
                if (author != null)
                {
                    authorId = author.Id;
                }
            }
            
            Posts = await postService.GetPostsPagedAsync(PageNumber, PageSize, new PostFilterOptions()
            {
                ForumId = ForumId,
                AuthorId = authorId,
                Pinned = Pinned,
                Locked = Locked,
                Published = Published,
                Title = Title,
                Content = ContentSearch
            }, "created", false);
            return Page();
        }
    }
}
