using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Forums 
{ 
    public class IndexModel(IForumService forumService) : PageModel
    {
        public ForumIndexPageDto ForumIndexPageDto { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var dtoResult = await forumService.GetForumIndexPageDtoAsync();
            if (dtoResult.IsFailed)
            {
                ModelState.AddModelError("","An error occurred.");
                return Page();
            }
            
            ForumIndexPageDto = dtoResult.Value;
            return Page();
        }
    }
}
