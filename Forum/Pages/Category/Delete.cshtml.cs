using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Category;

public class DeleteModel(ICategoryService categoryService) : PageModel
{
    public async Task<IActionResult> OnGet(int id)
    {
        await categoryService.DeleteCategoryByIdAsync(id);
        return RedirectToPage("/Admin/Categories");
    }
}