using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Services;

namespace NETForum.Pages.Category
{
    public class CreateModel(ICategoryService categoryService) : PageModel
    {
        [BindProperty]
        public CategoryForm Form { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();

            
            try
            {
                var newCategory = Form.ToNewCategory();
                await categoryService.AddCategoryAsync(newCategory);
                return RedirectToPage("/Admin/Forums/Index");
            } 
            catch(UniqueConstraintException ex)
            {
                switch (ex.ConstraintName.Split("_").Last())
                {
                    case "Name":
                        var error = $"Name '{Form.Name}' is already taken";
                        ModelState.AddModelError("", error);
                        break;
                }
                return Page();
            }
        }
    }
}
