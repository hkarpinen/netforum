using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Services;
using NETForum.Services.Criteria;

namespace NETForum.Pages.Admin;

[Authorize(Roles = "Admin")]
public class Categories(ICategoryService categoryService) : PageModel
{
    public PagedResult<Models.Category> CategoriesResult { get; set; } = new();

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
        CategoriesResult = await categoryService.GetCategoriesPagedAsync(
            PageNumber,
            PageSize,
            new CategorySearchCriteria
            {
                Name = Name,
                Published = Published
            }
        );
        return Page();
    }
}