using NETForum.Pages.Forums;

namespace NETForum.Extensions;

public static class ForumExtension
{
    public static ForumForm ToForm(this Models.Forum forum)
    {
        return new ForumForm
        {
            Id = forum.Id,
            Name = forum.Name,
            Description = forum.Description,
            CategoryId = forum.CategoryId,
            ParentForumId = forum.ParentForumId,
            Published = forum.Published
        };
    }
}