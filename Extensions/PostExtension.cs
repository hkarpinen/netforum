using NETForum.Pages.Posts;

namespace NETForum.Extensions;

public static class PostExtension
{
    // TODO: Need to populate other properties like IsLocked, IsPinned  ect.
    public static PostForm ToForm(this Models.Post post)
    {
        return new PostForm
        {
            Id = post.Id,
            ForumId = post.ForumId,
            AuthorId = post.AuthorId,
            Title = post.Title,
            Content = post.Content
        };
    }
}