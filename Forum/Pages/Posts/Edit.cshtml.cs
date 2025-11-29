using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Constants;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    [Authorize(Roles = "Admin,Member")]
    public class EditModel(IPostService postService, IUserService userService) : PageModel
    {
        [BindProperty]
        public EditPostDto EditPostDto { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Handle scenario where username is null.
            if (User.Identity?.Name == null) return RedirectToPage(PageRoutes.Login);

            var getEditPostDtoResult = await postService.GetPostForEditAsync(id);
            if (getEditPostDtoResult.IsFailed) return NotFound();
            var post = getEditPostDtoResult.Value;
            var authorLookupResult = await userService.GetUserAsync(post.AuthorId);
            
            // TODO: Unsure what is appropriate here.
            if (authorLookupResult.IsFailed) return RedirectToPage("/Error");
            
            // Forbid a user who is not the author from editing a post that is not theirs.
            if (User.Identity.Name != authorLookupResult.Value.UserName) return Forbid();

            EditPostDto = getEditPostDtoResult.Value;
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var getEditPostDtoResult = await postService.GetPostForEditAsync(id);
            if (getEditPostDtoResult.IsFailed) return NotFound();
            
            
            var authorLookupResult = await userService.GetUserAsync(getEditPostDtoResult.Value.AuthorId);
            if (authorLookupResult.IsFailed) return NotFound();
            if (User.Identity.Name != authorLookupResult.Value.UserName) return Forbid();
            
            var updatePostResult = await postService.UpdatePostAsync(id, EditPostDto);
            if (updatePostResult.IsSuccess) return RedirectToPage(PageRoutes.PostView, new { id });
            
            // Handle update error
            foreach (var error in updatePostResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }
    }
}
