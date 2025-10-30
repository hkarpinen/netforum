using NETForum.Pages.Category;

namespace NETForum.Extensions;

public static class CategoryExtension
{
    public static CategoryForm ToForm(this Models.Category category)
    {
        return new CategoryForm
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Published = category.Published,
            SortOrder = category.SortOrder
        };
    }
}