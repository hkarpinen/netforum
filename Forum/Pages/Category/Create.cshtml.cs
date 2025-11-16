using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Category
{
    public class CreateModel(ICategoryService categoryService) : PageModel
    {
        [BindProperty]
        public CreateCategoryDto CreateCategoryDto { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var categoryCreateResult = await categoryService.AddCategoryAsync(CreateCategoryDto);
            if(categoryCreateResult.IsSuccess) return RedirectToPage("/Admin/Categories");
            
            // Create result was not successful.
            ModelState.AddModelError(string.Empty, categoryCreateResult.Error.Message);
            return Page();
        }
    }
}
