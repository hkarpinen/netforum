using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Services;

namespace NETForum.Pages.Category;

public class EditModel(ICategoryService categoryService) : PageModel
{
    [BindProperty]
    public CategoryForm Form { get; set; } = new();
    
    public async Task<IActionResult> OnGet(int id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();  
        Form = category.ToForm();
        return Page();
    }

    // TODO: Implement updating the existing category. 
    public async Task<IActionResult> OnPostAsync()
    {
        throw new NotImplementedException();
    }
}