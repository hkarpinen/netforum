using NETForum.Models;
using NETForum.Pages.Replies;

namespace NETForum.Extensions;

public static class ReplyFormExtension
{
    public static Reply ToNewReply(this ReplyForm replyForm)
    {
        return new Reply
        {
            PostId = replyForm.PostId,
            AuthorId = replyForm.AuthorId,
            Content = replyForm.Content,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };
    }
}