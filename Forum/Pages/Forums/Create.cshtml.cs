using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Forums
{
    public class CreateModel(
        IForumService forumService,
        ICategoryService categoryService
        ) : PageModel {
        
        [BindProperty]
        public CreateForumDto CreateForumDto { get; set; } = new();

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ParentForums { get; set; }  = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            ParentForums = await forumService.GetSelectListItemsAsync();
            Categories = await categoryService.GetCategorySelectListItems();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ParentForums = await forumService.GetSelectListItemsAsync();
            if(!ModelState.IsValid) return Page();

            try
            {
                await forumService.AddForumAsync(CreateForumDto);
                return RedirectToPage("/Admin/Forums/Index");
            } 
            catch (UniqueConstraintException ex)
            {
                switch (ex.ConstraintName.Split("_").Last())
                {
                    case "Name":
                        var error = $"Name '{CreateForumDto.Name}' is already taken"; 
                        ModelState.AddModelError("", error);
                        break;
                }
                return Page();
            }
        }
    }
}
