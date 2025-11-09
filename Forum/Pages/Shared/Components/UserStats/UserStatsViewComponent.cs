using Microsoft.AspNetCore.Mvc;
using NETForum.Services;

namespace NETForum.Pages.Shared.Components.UserStats
{
    public class UserStatsViewComponent(
        IUserService userService,
        IPostService postService,
        IReplyService replyService)
        : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int authorId)
        {
            var totalPostCount = await postService.GetTotalPostCountByAuthorAsync(authorId);
            var totalReplyCount = await replyService.GetTotalReplyCountAsync(authorId);
            var joinedOn = await userService.GetUserJoinedDate(authorId);
            
            return View(new UserStatsModel
            {
                TotalPostsCount = totalPostCount,
                TotalRepliesCount = totalReplyCount,
                JoinedOn = joinedOn
            });
        }
    }
}
