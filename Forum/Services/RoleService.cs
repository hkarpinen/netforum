using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Filters;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Services.Specifications;
using FluentResults;

namespace NETForum.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<SelectListItem>> GetSelectItemsAsync();
        Task<IdentityResult> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<IdentityResult?> DeleteRoleAsync(int id);
        Task<IdentityResult?> UpdateRoleAsync(EditRoleDto editRoleDto);
        Task<Result<EditRoleDto>> GetRoleForEditAsync(int id);
        Task<PagedResult<Role>> GetRolesPagedAsync(RoleFilterOptions roleFilterOptions);
    }

    public class RoleService(
        RoleManager<Role> roleManager,
        IMapper mapper,
        AppDbContext appDbContext
        ) : IRoleService
    {
        public async Task<PagedResult<Role>> GetRolesPagedAsync(RoleFilterOptions roleFilterOptions) {
            var roleSearchSpec = new RoleSearchSpec(roleFilterOptions);

            var totalRoleCount = await appDbContext.Roles.CountAsync();
            var roles = await appDbContext.Roles
                .WithSpecification(roleSearchSpec)
                .ToListAsync();
            var pagedResult = new PagedResult<Role>
            {
                PageNumber = roleFilterOptions.PageNumber,
                PageSize = roleFilterOptions.PageSize,
                TotalCount = totalRoleCount,
                Items = roles
            };
            return pagedResult;
        }

        public async Task<Result<EditRoleDto>> GetRoleForEditAsync(int id)
        {
            var role = await appDbContext.Roles.Where(r => r.Id == id).FirstOrDefaultAsync();
            if (role == null)
            {
                return Result.Fail<EditRoleDto>("Role not found");
            }
            var editRoleDto = mapper.Map<EditRoleDto>(role);
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
            var role = mapper.Map<Role>(createRoleDto);
            return await roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult?> DeleteRoleAsync(int id)
        {
            var role = await appDbContext.Roles.FindAsync(id);
            if (role == null) return null;
            return await roleManager.DeleteAsync(role);
        }

        public async Task<IdentityResult?> UpdateRoleAsync(EditRoleDto editRoleDto)
        {
            var role = await appDbContext.Roles.FindAsync(editRoleDto.Id);
            if(role == null) return null;
            mapper.Map(editRoleDto, role);
            var result = await roleManager.UpdateAsync(role);
            return result;
        }
    }
}
