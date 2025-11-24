using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    [Authorize]
    public class CreateModel(IPostService postService) : PageModel
    {
        [BindProperty]
        public CreatePostDto CreatePostDto { get; set; } = new();

        public async Task<IActionResult> OnPostAsync(int forumId)
        {
            if (!ModelState.IsValid) return Page();
            
            var username = User.Identity?.Name!;
            var addPostResult = await postService.AddPostAsync(username, forumId, CreatePostDto);
            if(addPostResult.IsSuccess) return RedirectToPage("/Posts/Details", new { id = addPostResult.Value.Id });

            // Adding the post failed, add the error to the ModelState.
            foreach (var error in addPostResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }
    }
}
