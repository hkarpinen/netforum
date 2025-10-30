using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Services;

namespace NETForum.Pages.Forums;

public class EditModel(IForumService forumService, ICategoryService categoryService)
    : PageModel
{
    [BindProperty] 
    public ForumForm Form { get; set; } = new();
    
    public async Task<IActionResult> OnGet(int id)
    {
        var forum = await forumService.GetForumByIdAsync(id);
        if (forum == null) return NotFound();

        Form = forum.ToForm();
        
        // TODO: Rename category service method to be async.
        Form.AvailableCategories = await categoryService.GetCategorySelectListItems();
        Form.AvailableParentForums = await forumService.GetSelectListItemsAsync();
        
        return Page();
    }

    public async Task<IActionResult> OnPost(int id)
    {
        if (!ModelState.IsValid) return Page();
        try
        {
            var forum = await forumService.GetForumByIdAsync(id);
            if (forum == null) return NotFound();
            
            await forumService.UpdateForum(
                Form.MapToForum(forum)
            );
            
            return RedirectToPage("/Admin/Forums/Index");
        } 
        catch (UniqueConstraintException ex)
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