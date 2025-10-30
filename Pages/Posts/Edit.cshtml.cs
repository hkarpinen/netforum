using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    [Authorize(Roles = "Admin,Member")]
    public class EditModel(IPostService postService) : PageModel
    {
        [BindProperty]
        public PostForm Form { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fake data for the form, need to use a service to fetch real data.
            var post = await postService.GetPostAsync(id);
            if(post == null)
            {
                return NotFound();
            }

            if(User.Identity?.Name == null)
            {
                return RedirectToPage("/Account/Login");
            }

            if(post.Author == null) throw new Exception("Post Author Not Found");
            if(User.Identity.Name != post.Author.UserName)
            {
                return Forbid();
            }

            Form = post.ToForm();

            return Page();
        }

        // TODO: Need to implement updating an existing post. 
        public async Task<IActionResult> OnPostAsync()
        {
            throw new NotImplementedException();
        }
    }
}
