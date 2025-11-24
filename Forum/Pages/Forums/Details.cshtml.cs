using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Forums
{
    public class DetailsModel(IForumService forumService, IPostService postService) : PageModel
    {
        public ForumPageDto ForumPageDto { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var forumPageResult = await forumService.GetForumPageDtoAsync(id);
            if (forumPageResult.IsFailed) return NotFound();
            
            ForumPageDto = forumPageResult.Value;
            
            return Page();
        }
    }
}
