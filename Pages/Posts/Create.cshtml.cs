using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    [Authorize(Roles = "Admin,Member")]
    public class CreateModel(
        IUserService userService,
        IPostService postService) : PageModel
    {
        [BindProperty]
        public PostForm Form { get; set; } = new();

        private async Task PopulateForm(int forumId)
        {
            var user = await userService.GetUserAsync(User.Identity!.Name!);
            Form.ForumId = forumId;
            Form.AuthorId = user.Id;
            Form.AuthorName = user.UserName;
            Form.IsLocked = false;
            Form.IsPinned = false;
        }

        public async Task OnGetAsync(int forumId)
        {
            await PopulateForm(forumId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await PopulateForm(Form.ForumId);   
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var newPost = Form.ToNewPost();
                
                var result = await postService.CreatePostAsync(newPost);
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
