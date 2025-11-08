using AutoMapper;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Forums;

public class EditModel(
    IForumService forumService, 
    ICategoryService categoryService,
    IMapper mapper)
    : PageModel
{
    [BindProperty] 
    public EditForumDto EditForumDto { get; set; } = new();
    
    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> ParentForums { get; set; } = new List<SelectListItem>();
    
    public async Task<IActionResult> OnGet(int id)
    {
        var forum = await forumService.GetForumByIdAsync(id);
        if (forum == null) return NotFound();
        EditForumDto = mapper.Map<EditForumDto>(forum);
        
        // TODO: Rename category service method to be async.
        Categories = await categoryService.GetCategorySelectListItems();
        ParentForums = await forumService.GetSelectListItemsAsync();
        
        return Page();
    }

    public async Task<IActionResult> OnPost(int id)
    {
        if (!ModelState.IsValid) return Page();
        try
        {
            var forum = await forumService.GetForumByIdAsync(id);
            if (forum == null) return NotFound();
            await forumService.UpdateForum(EditForumDto);
            return RedirectToPage("/Admin/Forums/Index");
        } 
        catch (UniqueConstraintException ex)
        {
            switch (ex.ConstraintName.Split("_").Last())
            {
                case "Name":
                    var error = $"Name '{EditForumDto.Name}' is already taken"; 
                    ModelState.AddModelError("", error);
                    break;
            }
            return Page();
        }
    }
}