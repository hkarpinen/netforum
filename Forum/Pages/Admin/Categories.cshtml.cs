using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CategoriesModel(ICategoryService categoryService) : PageModel
{
    public PagedResult<Models.Entities.Category> CategoriesResult { get; set; } = new();

    [BindProperty(SupportsGet = true)] 
    public int PageNumber { get; set; } = 1;
    
    [BindProperty(SupportsGet = true)] 
    public int PageSize { get; set; } = 10;
    
    [BindProperty(SupportsGet = true)]
    public string? Name { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public bool Published { get; set; } = true;
    
    public async Task<IActionResult> OnGetAsync()
    {
        CategoriesResult = await categoryService.GetCategoriesWithForumsPagedAsync(
            PageNumber,
            PageSize,
            new CategoryFilterOptions
            {
                Name = Name,
                Published = Published
            }
        );
        return Page();
    }
}