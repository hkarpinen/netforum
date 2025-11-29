using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Constants;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Admin.Categories
{
    public class CreateModel(ICategoryService categoryService) : PageModel
    {
        [BindProperty]
        public CreateCategoryDto CreateCategoryDto { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var categoryCreateResult = await categoryService.AddCategoryAsync(CreateCategoryDto);
            if(categoryCreateResult.IsSuccess) return RedirectToPage(PageRoutes.ManageCategories);
            
            // Create result was not successful.
            foreach (var error in categoryCreateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }
    }
}
