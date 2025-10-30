using NETForum.Pages.Category;

namespace NETForum.Extensions;

public static class CategoryFormExtension
{
    public static Models.Category ToNewCategory(this CategoryForm categoryForm)
    {
        return new Models.Category
        {
            Name = categoryForm.Name,
            Description = categoryForm.Description,
            SortOrder = categoryForm.SortOrder,
            Published = categoryForm.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}