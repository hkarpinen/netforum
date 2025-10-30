using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Extensions;
using NETForum.Services;

namespace NETForum.Pages.Forums
{
    public class CreateModel(
        IForumService forumService,
        ICategoryService categoryService)
        : PageModel
    {
        [BindProperty]
        public ForumForm Form { get; set; } = new();

        public async Task OnGetAsync()
        {
            Form.AvailableParentForums = await forumService.GetSelectListItemsAsync();
            Form.AvailableCategories = await categoryService.GetCategorySelectListItems();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Form.AvailableParentForums = await forumService.GetSelectListItemsAsync();
            if(!ModelState.IsValid) return Page();

            try
            {
                var newForum = Form.ToForum();
                await forumService.CreateForumAsync(newForum);
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
}
