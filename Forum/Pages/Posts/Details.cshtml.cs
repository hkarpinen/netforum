using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    public class DetailModel(
        IForumService forumService,
        IPostService postService,
        IReplyService replyService,
        IUserService userService
    ) : PageModel 
    {
        public Post Post { get; set; }
        public User AuthenticatedUser { get; set; }
        public IEnumerable<Reply> Replies { get; set; } = new List<Reply>();
        public IEnumerable<BreadcrumbItemModel> ForumBreadcrumbs { get; set; } = new List<BreadcrumbItemModel>();
        public int AuthorTotalPosts { get; set; }
        public int AuthorTotalReplies { get; set; }
        public DateTime AuthorJoinedOn { get; set; }
        public bool UserIsAuthor { get; set; }

        [BindProperty]
        public CreatePostReplyDto CreatePostReplyDto { get; set; } = new();

        public async Task<bool> LoadPostAndAuthorData(int postId)
        {
            if (User.Identity?.Name == null) return false;
            var userLookupResult = await userService.GetByUsernameAsync(User.Identity.Name);
            if (userLookupResult.IsFailure) return false;
            AuthenticatedUser = userLookupResult.Value;
            var postLookupResult = await postService.GetPostWithAuthorAndRepliesAsync(postId);
            if (postLookupResult.IsFailure) return false;
            Post = postLookupResult.Value;
            Replies = postLookupResult.Value.Replies;
            AuthorTotalPosts = await postService.GetTotalPostCountByAuthorAsync(Post.AuthorId);
            AuthorTotalReplies = await replyService.GetTotalReplyCountAsync(Post.AuthorId);
            UserIsAuthor = postLookupResult.Value.AuthorId == userLookupResult.Value.Id;
            ForumBreadcrumbs = await forumService.GetForumBreadcrumbItems(Post.ForumId);
            AuthorJoinedOn = Post.Author!.CreatedAt;
            return true;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var loadSuccessful = await LoadPostAndAuthorData(id);
            return loadSuccessful ? Page() : RedirectToPage("/Error");
        }

        public async Task<IActionResult> OnPostAddReplyAsync(int id)
        {
            var loadSuccessful = await LoadPostAndAuthorData(id);
            if (!loadSuccessful) return RedirectToPage("/Error");
            if (!ModelState.IsValid) return Page();
            var addReplyResult = await replyService.AddReplyAsync(id, AuthenticatedUser.Id, CreatePostReplyDto);
            if (addReplyResult.IsSuccess) return RedirectToPage();
            
            // Reply add operation failed
            ModelState.AddModelError("", addReplyResult.Error.Message);
            return Page();
        }
    }
}
