using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Constants;
using NETForum.Services;

namespace NETForum.Pages.Admin.Categories;

public class DeleteModel(ICategoryService categoryService) : PageModel
{
    public async Task<IActionResult> OnGetAsync(int id)
    {
        await categoryService.DeleteCategoryByIdAsync(id);
        return RedirectToPage(PageRoutes.ManageCategories);
    }
}