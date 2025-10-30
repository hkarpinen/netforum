using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Pages.Replies;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Services;

namespace NETForum.Pages.Posts
{
    public class DetailModel(
        IForumService forumService,
        IPostService postService,
        IReplyService replyService,
        IUserService userService)
        : PageModel
    {
        public Post Post { get; set; }
        public IEnumerable<Reply> Replies { get; set; } = new List<Reply>();
        public IEnumerable<BreadcrumbItemModel> ForumBreadcrumbs { get; set; } = new List<BreadcrumbItemModel>();
        public int AuthorTotalPosts { get; set; }
        public int AuthorTotalReplies { get; set; }
        public DateTime AuthorJoinedOn { get; set; }
        public bool UserIsAuthor { get; set; }

        [BindProperty]
        public ReplyForm ReplyForm { get; set; } = new();

        private async Task PopulateAuthorStats(int authorId)
        {
            AuthorTotalPosts = await postService.GetTotalPostCountAsync(authorId);
            AuthorTotalReplies = await replyService.GetTotalReplyCountAsync(authorId);
        }

        private async Task<bool> PopulateForm(int id)
        {
            var post = await postService.GetPostAsync(id);
            if (post == null) return false;

            Post = post;
            Replies = await replyService.GetRepliesAsync(Post.Id);
            await PopulateAuthorStats(Post.AuthorId);
            
            // If the user is logged in, prepare the reply form.
            if (User.Identity is { Name: not null })
            {
                var user = await userService.GetUserAsync(User.Identity.Name);
                UserIsAuthor = user.Id == post.AuthorId;

                ReplyForm.PostId = id;
                ReplyForm.AuthorName = user.UserName;
                ReplyForm.AuthorId = user.Id;
                AuthorJoinedOn = user.CreatedAt;
            }

            ForumBreadcrumbs = await forumService.GetForumBreadcrumbItems(Post.ForumId);

            return true;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var success = await PopulateForm(id);
            if (!success) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAddReplyAsync(int id)
        {
            var success = await PopulateForm(id);
            if (!success) return NotFound();
            if (!ModelState.IsValid) return Page();
            
            // Add the reply to the database and redirect to the same page to show the new reply.
            var newReply = ReplyForm.ToNewReply();
            await replyService.AddReplyAsync(id, newReply);
            return RedirectToPage();
        }
    }
}
