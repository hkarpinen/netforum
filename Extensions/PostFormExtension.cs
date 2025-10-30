using NETForum.Pages.Posts;

namespace NETForum.Extensions;

public static class PostFormExtension
{
    // TODO: Move view count, reply count, created/updated at dates outside this method.
    // This method should not be responsible for providing default values.
    public static Models.Post ToNewPost(this PostForm postForm)
    {
        return new Models.Post
        {
            ForumId = postForm.ForumId,
            AuthorId = postForm.AuthorId,
            IsPinned = postForm.IsPinned,
            IsLocked = postForm.IsLocked,
            Published = postForm.Published,
            Title = postForm.Title,
            Content = postForm.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ViewCount = 0,
            ReplyCount = 0
        };
    }
}