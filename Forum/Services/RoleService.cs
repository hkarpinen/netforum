using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;
using NETForum.Repositories.Filters;

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
        Task<PagedResult<Role>> GetRolesPagedAsync(
            int pageNumber, 
            int pageSize, 
            RoleFilterOptions roleFilterOptions,
            string? sortBy,
            bool ascending
        );
    }

    public class RoleService(
        RoleManager<Role> roleManager,
        IMapper mapper,
        IRoleRepository roleRepository)
        : IRoleService
    {
        public async Task<PagedResult<Role>> GetRolesPagedAsync(
            int pageNumber, 
            int pageSize, 
            RoleFilterOptions roleFilterOptions,
            string? sortBy,
            bool ascending
        ) {
            var repositoryPagedQueryOptions = new PagedRepositoryQueryOptions<RoleFilterOptions>()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = roleFilterOptions,
                SortBy = sortBy,
                Ascending = ascending
            };
            return await roleRepository.GetAllPagedAsync(repositoryPagedQueryOptions);
        }

        // TODO: Rename to GetAllRolesAsync()
        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            var queryOptions = new RepositoryQueryOptions<RoleFilterOptions>();
            return await roleRepository.GetAllAsync(queryOptions);
        }

        public async Task<Role?> GetRoleAsync(int id)
        {
            return await roleRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectItemsAsync()
        {
            var queryOptions = new RepositoryQueryOptions<RoleFilterOptions>();
            var allRoles = await roleRepository.GetAllAsync(queryOptions);
            return allRoles.Select(r => new SelectListItem
            {
                // TODO: Value should be the ID, not the Name value.
                Value = r.Name,
                Text = r.Name
            });
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
