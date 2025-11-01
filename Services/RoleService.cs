using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Pages.Roles;
using NETForum.Services.Criteria;

namespace NETForum.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<IEnumerable<SelectListItem>> GetSelectItemsAsync();
        Task<Role?> GetRoleAsync(int id);
        Task<IdentityResult> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<IdentityResult?> DeleteRoleAsync(int id);
        Task<IdentityResult?> UpdateRoleAsync(EditRoleDto editRoleDto);
        Task<PagedResult<Role>> GetRolesPagedAsync(int pageNumber, int pageSize, RoleSearchCriteria roleSearchCriteria);
    }

    public class RoleService(
        AppDbContext context,
        RoleManager<Role> roleManager,
        IMapper mapper)
        : IRoleService
    {
        public async Task<PagedResult<Role>> GetRolesPagedAsync(int pageNumber, int pageSize, RoleSearchCriteria roleSearchCriteria)
        {
            var query = context.Roles
                .WhereName(roleSearchCriteria.Name)
                .WhereDescription(roleSearchCriteria.Description);

            var totalItems = await query.CountAsync();
            var roles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Role>
            {
                Items = roles,
                TotalCount = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await context.Roles.ToListAsync();
        }

        public async Task<Role?> GetRoleAsync(int id)
        {
            return await context.Roles
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectItemsAsync()
        {
            return await context.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToListAsync();
        }

        public async Task<IdentityResult> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            var role = mapper.Map<Role>(createRoleDto);
            return await roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult?> DeleteRoleAsync(int id)
        {
            var role = await GetRoleAsync(id);
            if (role == null) return null;
            return await roleManager.DeleteAsync(role);
        }

        public async Task<IdentityResult?> UpdateRoleAsync(EditRoleDto editRoleDto)
        {
            var role = await GetRoleAsync(editRoleDto.Id);
            if(role == null) return null;
            mapper.Map(editRoleDto, role);
            var result = await roleManager.UpdateAsync(role);
            return result;
        }
    }
}
