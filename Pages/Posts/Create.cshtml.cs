using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    [Authorize(Roles = "Admin,Member")]
    public class CreateModel(IPostService postService) : PageModel
    {
        [BindProperty]
        public CreatePostDto CreatePostDto { get; set; } = new();

        public async Task<IActionResult> OnPostAsync(int forumId)
        {
            // If the form state is invalid, show validation errors.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle case where username is null.
            var username = User.Identity?.Name;
            if (username == null)
            {
                ModelState.AddModelError(string.Empty, "You must have a user name to post.");
                return Page();
            }

            // Create the post and catch any exceptions
            try
            {
                var result = await postService.CreatePostAsync(username, forumId, CreatePostDto);
                return RedirectToPage("/Posts/Details", new { id = result.Entity.Id });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }
    }
}
