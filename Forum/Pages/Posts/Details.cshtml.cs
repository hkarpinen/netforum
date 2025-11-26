using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    public class DetailModel(
        IPostService postService,
        IReplyService replyService,
        IUserService userService
    ) : PageModel
    {
        public PostPageDto? PostPageDto { get; set; }

        [BindProperty]
        public CreatePostReplyDto CreatePostReplyDto { get; set; } = new();
        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if(User.Identity?.Name == null) return Challenge();
            
            var postPageDtoResult = await postService.GetPostPageDto(id, User.Identity.Name);
            if (postPageDtoResult.IsSuccess)
            {
                PostPageDto = postPageDtoResult.Value;
                return Page();
            }
            
            foreach (var error in postPageDtoResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddReplyAsync(int id)
        {
            if(User.Identity?.Name == null) return Challenge();
            if (!ModelState.IsValid) return Page();
            var currentUserLookup = await userService.GetByUsernameAsync(User.Identity.Name);
            if (currentUserLookup.IsFailed) return Forbid();
            
            var postPageDtoResult = await postService.GetPostPageDto(id, User.Identity.Name);
            if (postPageDtoResult.IsFailed)
            {
                foreach (var error in postPageDtoResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
                return Page();
            }
            
            PostPageDto = postPageDtoResult.Value;
            
            var addReplyResult = await replyService.AddReplyAsync(id, currentUserLookup.Value.Id, CreatePostReplyDto);
            if (addReplyResult.IsSuccess) return RedirectToPage();
            
            // Reply add operation failed
            foreach (var error in addReplyResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }
    }
}
