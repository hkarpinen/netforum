using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Category;

public class EditModel(ICategoryService categoryService) : PageModel
{
    [BindProperty]
    public EditCategoryDto EditCategoryDto { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var editCategoryDtoResult = await categoryService.GetCategoryForEditAsync(id);
        if(editCategoryDtoResult.IsFailed) return NotFound();
        EditCategoryDto = editCategoryDtoResult.Value;
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(int id)
    {
        var editCategoryDtoResult = await categoryService.GetCategoryForEditAsync(id);
        if(editCategoryDtoResult.IsFailed) return NotFound();
        EditCategoryDto = editCategoryDtoResult.Value;
        if(!ModelState.IsValid) return Page();
        
        var updateCategoryResult = await categoryService.UpdateCategoryAsync(id, EditCategoryDto);
        if(updateCategoryResult.IsSuccess)  return RedirectToPage("/Admin/Categories");
        
        // An error occurred updating the category. 
        foreach (var error in updateCategoryResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }
        return Page();
    }
}