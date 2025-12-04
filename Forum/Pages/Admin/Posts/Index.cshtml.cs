using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Admin.Posts
{
    [Authorize(Roles = "Admin")]
    public class IndexModel(IPostService postService) : PageModel
    {
        public PagedList<Post> Posts { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        
        [BindProperty(SupportsGet = true)]
        public int? ForumId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? AuthorName { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool? Pinned { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool? Locked { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool? Published { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Title { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Content { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
    }
}
