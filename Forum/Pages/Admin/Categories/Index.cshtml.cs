using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Filters;
using NETForum.Models;
using NETForum.Services;
using NETForum.Models.Entities;

namespace NETForum.Pages.Admin.Categories;

[Authorize(Roles = "Admin")]
public class CategoriesModel(ICategoryService categoryService) : PageModel
{
    public PagedList<Category> CategoriesResult { get; set; } = new();

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
        CategoriesResult = await categoryService.GetCategoriesPagedAsync(new CategoryFilterOptions
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            Name = Name,
            Published = true
        });
        return Page();
    }
}