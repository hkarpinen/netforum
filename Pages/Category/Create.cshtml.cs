using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;

namespace NETForum.Pages.Category
{
    public class CreateModel(ICategoryService categoryService) : PageModel
    {
        [BindProperty]
        public CreateCategoryDto CreateCategoryDto { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            
            try
            {
                await categoryService.AddCategoryAsync(CreateCategoryDto);
                return RedirectToPage("/Admin/Forums/Index");
            } 
            catch(UniqueConstraintException ex)
            {
                switch (ex.ConstraintName.Split("_").Last())
                {
                    case "Name":
                        var error = $"Name '{CreateCategoryDto.Name}' is already taken";
                        ModelState.AddModelError("", error);
                        break;
                }
                return Page();
            }
        }
    }
}
