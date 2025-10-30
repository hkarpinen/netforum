using NETForum.Pages.Forums;

namespace NETForum.Extensions;

public static class ForumFormExtension
{
    public static Models.Forum ToForum(this ForumForm form)
    {
        return new Models.Forum
        {
            Name = form.Name,
            Description = form.Description,
            Published = form.Published,
            ParentForumId = form.ParentForumId,
            CategoryId = form.CategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Models.Forum MapToForum(this ForumForm form, Models.Forum forum)
    {
        forum.Name = form.Name;
        forum.Description = form.Description;
        forum.Published = form.Published;
        forum.ParentForumId = form.ParentForumId;
        forum.CategoryId = form.CategoryId;
        forum.UpdatedAt = DateTime.UtcNow;
        return forum;
    }
}