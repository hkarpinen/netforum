using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Constants;
using NETForum.Models.DTOs;
using NETForum.Services;

namespace NETForum.Pages.Admin.Forums
{
    public class CreateModel(IForumService forumService, ICategoryService categoryService) : PageModel 
    {
        
        [BindProperty]
        public CreateForumDto CreateForumDto { get; set; } = new();
        public IEnumerable<SelectListItem> CategorySelectListItems { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ParentForumSelectListItems { get; set; }  = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            ParentForumSelectListItems = await forumService.GetForumSelectListItemsAsync();
            CategorySelectListItems = await categoryService.GetCategorySelectListItemsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ParentForumSelectListItems = await forumService.GetForumSelectListItemsAsync();
            CategorySelectListItems = await categoryService.GetCategorySelectListItemsAsync();
            
            if(!ModelState.IsValid) return Page();
            var addForumResult = await forumService.AddForumAsync(CreateForumDto);
            if(addForumResult.IsSuccess) return RedirectToPage(PageRoutes.ManageForums);

            // Handle forum add errors
            foreach (var error in addForumResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return Page();
        }
    }
}
