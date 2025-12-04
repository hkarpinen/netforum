using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Filters;
using NETForum.Models;
using NETForum.Models.Entities;
using NETForum.Services;

namespace NETForum.Pages.Admin.Roles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel(IRoleService roleService) : PageModel
    {
        public PagedList<Role> Roles { get; set; } = [];
        
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        
        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? Description { get; set; }

        public async Task OnGetAsync()
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
