using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services.Specifications;
using FluentResults;
using NETForum.Models;

namespace NETForum.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<SelectListItem>> GetSelectItemsAsync();
        Task<IdentityResult> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<IdentityResult?> DeleteRoleAsync(int id);
        Task<IdentityResult?> UpdateRoleAsync(int id, EditRoleDto editRoleDto);
        Task<Result<EditRoleDto>> GetRoleForEditAsync(int id);
        Task<PagedList<Role>> GetRolesPagedAsync(RoleFilterOptions roleFilterOptions);
    }

    public class RoleService(
        RoleManager<Role> roleManager,
        AppDbContext appDbContext
        ) : IRoleService
    {
        public async Task<PagedList<Role>> GetRolesPagedAsync(RoleFilterOptions roleFilterOptions) {
            var roleSearchSpec = new RoleSearchSpec(roleFilterOptions);

            var totalRoleCount = await appDbContext.Roles.CountAsync();
            var roles = await appDbContext.Roles
                .WithSpecification(roleSearchSpec)
                .ToListAsync();

            var pagedList = new PagedList<Role>(
                roles,
                totalRoleCount,
                roleFilterOptions.PageNumber,
                roleFilterOptions.PageSize
            );
            return pagedList;
        }

        public async Task<Result<EditRoleDto>> GetRoleForEditAsync(int id)
        {
            var role = await appDbContext.Roles.Where(r => r.Id == id).FirstOrDefaultAsync();
            if (role == null)
            {
                return Result.Fail<EditRoleDto>("Role not found");
            }
            
            // Map Role to Edit DTO
            var editRoleDto = new EditRoleDto
            {
                Name = role.Name ?? string.Empty,
                Description = role.Description
            };
            
            return Result.Ok(editRoleDto);
        }
        
        public async Task<IEnumerable<SelectListItem>> GetSelectItemsAsync()
        {
            return await appDbContext.Roles.Select(r => new SelectListItem
            {
                // TODO: Value should be the ID, not the Name value.
                Value = r.Name,
                Text = r.Name
            }).ToListAsync();
        }

        public async Task<IdentityResult> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            // Map Create DTO to role
            var role = new Role
            {
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                NormalizedName = createRoleDto.Name.ToUpper()
            };
            
            return await roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult?> DeleteRoleAsync(int id)
        {
            var role = await appDbContext.Roles.FindAsync(id);
            if (role == null) return null;
            return await roleManager.DeleteAsync(role);
        }

        public async Task<IdentityResult?> UpdateRoleAsync(int id, EditRoleDto editRoleDto)
        {
            var role = await appDbContext.Roles.FindAsync(id);
            if(role == null) return null;
            
            // Map Edit DTO to role
            role.Name = editRoleDto.Name;
            role.NormalizedName = editRoleDto.Name.ToUpper();
            role.Description = editRoleDto.Description;
            var result = await roleManager.UpdateAsync(role);
            return result;
        }
    }
}
