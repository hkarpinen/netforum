using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.Entities;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Pages.Shared.Components.ForumList;
using NETForum.Services;

namespace NETForum.Pages.Forums
{
    public class DetailsModel(IForumService forumService, IPostService postService) : PageModel
    {
        public IReadOnlyCollection<Post> Posts { get; set; } = new List<Post>();
        public IReadOnlyCollection<ForumListItemModel> ChildForumsWithPostAndReplies { get; set; } = new List<ForumListItemModel>();
        public IReadOnlyCollection<BreadcrumbItemModel> BreadcrumbItems { get; set; } = new List<BreadcrumbItemModel>();
        public Forum? Forum { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var forumLookupResult = await forumService.GetForumByIdAsync(id);
            if (forumLookupResult.IsFailure) return NotFound();
            
            // Forum exists
            ChildForumsWithPostAndReplies = await forumService.GetChildForumListItemsWithPostsAndRepliesAsync(id);
            Forum = forumLookupResult.Value;
            BreadcrumbItems = await forumService.GetForumBreadcrumbItems(id);
            Posts = await postService.GetPostsAsync(id);
            return Page();
        }
    }
}
