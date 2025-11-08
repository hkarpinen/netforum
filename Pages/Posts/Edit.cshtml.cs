using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    [Authorize(Roles = "Admin,Member")]
    public class EditModel(IPostService postService, IMapper mapper) : PageModel
    {
        [BindProperty]
        public EditPostDto EditPostDto { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fake data for the form, need to use a service to fetch real data.
            var post = await postService.GetPostAsync(id);
            if (post == null) return NotFound();
            
            // Handle scenario where username is null.
            if (User.Identity?.Name == null)
            {
                return RedirectToPage("/Account/Login");
            }

            if(post.Author == null) throw new Exception("Post Author Not Found");
            
            // Forbid a user who is not the author from editing a post that is not theirs.
            if(User.Identity.Name != post.Author.UserName)
            {
                return Forbid();
            }

            EditPostDto = mapper.Map<EditPostDto>(post);

            return Page();
        }

        // TODO: Need to implement updating an existing post. 
        public async Task<IActionResult> OnPostAsync()
        {
            throw new NotImplementedException();
        }
    }
}
