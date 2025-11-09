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

        private async Task<bool> LoadPostAndAuthorData(int postId)
        {
            // TODO: Might be better to the store the entire Author object instead of selecting properties to instantiate other properties. 
            if (User.Identity?.Name == null) return false;
            var authenticatedUser = await userService.GetUserAsync(User.Identity.Name);
            if (authenticatedUser == null) return false;
            AuthenticatedUser = authenticatedUser;
            var post = await postService.GetPostWithAuthorAndRepliesAsync(postId);
            if (post == null) return false;
            Post = post;
            Replies = await replyService.GetRepliesAsync(Post.Id);
            AuthorTotalPosts = await postService.GetTotalPostCountAsync(Post.AuthorId);
            AuthorTotalReplies = await replyService.GetTotalReplyCountAsync(Post.AuthorId);
            UserIsAuthor = post.AuthorId == authenticatedUser.Id;
            ForumBreadcrumbs = await forumService.GetForumBreadcrumbItems(Post.ForumId);
            
            // TODO: This is wrong, this is when the current user joined, not the Author. 
            AuthorJoinedOn = authenticatedUser.CreatedAt;

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
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("PostReplyCreateInputModel.Title", "Post reply creation failed");
                return Page();
            }
            
            // Add the reply to the database and redirect to the same page to show the new reply.
            try
            {
                await replyService.AddReplyAsync(id, AuthenticatedUser.Id, CreatePostReplyDto);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return Page();
            }
            return RedirectToPage();
        }
    }
}
