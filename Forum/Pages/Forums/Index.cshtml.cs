using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.Entities;
using NETForum.Pages.Shared.Components.ForumList;
using NETForum.Services;

namespace NETForum.Pages.Forums 
{ 
    public class IndexModel(
        IPostService postService,
        IUserService userService,
        IReplyService replyService,
        IForumService forumService)
        : PageModel
    {
        public IEnumerable<Post> LatestPosts { get; set; } = new List<Post>();
        public int TotalPostCount { get; set; }
        public int TotalMemberCount { get; set; }
        public int TotalReplyCount { get; set; }
        public User? NewestUser { get; set; }
        public IEnumerable<ForumListItemModel> RootForumItems { get; set; } = new List<ForumListItemModel>();

        public async Task OnGetAsync()
        {
            RootForumItems = await forumService.GetRootForumListItemsAsync();
            LatestPosts = await postService.GetLatestPostsAsync(5);
            TotalPostCount = await postService.GetTotalPostCountAsync();
            NewestUser = await userService.GetNewestUserAsync();
            TotalMemberCount = await userService.GetTotalUserCountAsync();
            TotalReplyCount = await replyService.GetTotalReplyCountAsync();
        }
    }
}
