using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Category;

public class EditModel(ICategoryService categoryService, IMapper mapper) : PageModel
{
    [BindProperty]
    public EditCategoryDto Form { get; set; } = new();
    
    public async Task<IActionResult> OnGet(int id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();  
        Form = mapper.Map<EditCategoryDto>(category);
        return Page();
    }

    // TODO: Implement updating the existing category. 
    public async Task<IActionResult> OnPostAsync()
    {
        throw new NotImplementedException();
    }
}