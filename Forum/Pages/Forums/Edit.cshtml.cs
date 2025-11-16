using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Forums;

public class EditModel(
    IForumService forumService, 
    ICategoryService categoryService)
    : PageModel
{
    [BindProperty] 
    public EditForumDto EditForumDto { get; set; } = new();
    public IEnumerable<SelectListItem> CategorySelectListItems { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> ParentForumSelectListItems { get; set; } = new List<SelectListItem>();
    
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var editForumDtoResult = await forumService.GetForumForEditAsync(id);
        if (editForumDtoResult.IsFailure) return NotFound();
        EditForumDto = editForumDtoResult.Value;
        CategorySelectListItems = await categoryService.GetCategorySelectListItemsAsync();
        ParentForumSelectListItems = await forumService.GetForumSelectListItemsAsync();
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var editForumDtoResult = await forumService.GetForumForEditAsync(id);
        if (editForumDtoResult.IsFailure) return NotFound();
        EditForumDto = editForumDtoResult.Value;
        
        CategorySelectListItems = await categoryService.GetCategorySelectListItemsAsync();
        ParentForumSelectListItems = await forumService.GetForumSelectListItemsAsync();
        
        if (!ModelState.IsValid) return Page();
        
        var updateForumResult = await forumService.UpdateForumAsync(id, EditForumDto);
        if(updateForumResult.IsSuccess) return RedirectToPage("/Admin/Forums/Index");
        
        // An error occurred.
        ModelState.AddModelError(string.Empty, updateForumResult.Error.Message);
        return Page();
    }
}