using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;
using NETForum.Pages.Shared.Components.Breadcrumbs;
using NETForum.Pages.Shared.Components.ForumList;
using NETForum.Services;

namespace NETForum.Pages.Forums
{
    public class DetailsModel(IForumService forumService, IPostService postService) : PageModel
    {
        public IEnumerable<Post> Posts { get; set; } = new List<Post>();
        public IEnumerable<ForumListItemModel> SubForumItems { get; set; } = new List<ForumListItemModel>();
        public IEnumerable<BreadcrumbItemModel> BreadcrumbItems { get; set; } = new List<BreadcrumbItemModel>();
        public int ForumId { get; set; }

        public async Task OnGetAsync(int id)
        {
            SubForumItems = await forumService.GetForumListItemsAsync(id);
            ForumId = id;
            BreadcrumbItems = await forumService.GetForumBreadcrumbItems(id);
            Posts = await postService.GetPostsAsync(id);
        }
    }
}
