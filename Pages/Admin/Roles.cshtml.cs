using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NETForum.Models;
using NETForum.Services;
using NETForum.Services.Criteria;

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
            Roles = await roleService.GetRolesPagedAsync(PageNumber, PageSize, new RoleSearchCriteria()
            {
                Name = Name,
                Description = Description
            });
        }
    }
}
