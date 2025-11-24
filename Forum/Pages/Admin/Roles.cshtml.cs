using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Filters;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class RolesModel(IRoleService roleService) : PageModel
    {
        public PagedResult<Role> Roles { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        
        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Description { get; set; }

        public async Task OnGet()
        {
            Roles = await roleService.GetRolesPagedAsync(new RoleFilterOptions
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Name = Name,
                Description = Description,
                SortBy = RoleSortBy.Name,
                Ascending = true
            });
        }
    }
}
