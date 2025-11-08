using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;
using NETForum.Repositories.Filters;

namespace NETForum.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetCategorySelectListItems();
        Task<Category> AddCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<Category?>GetCategoryByIdAsync(int id);
        Task UpdateCategoryAsync(EditCategoryDto editCategoryDto);
        Task DeleteCategoryByIdAsync(int id);
        Task<PagedResult<Category>> GetCategoriesWithForumsPagedAsync(int pageNumber, int pageSize, CategoryFilterOptions categoryFilterOptions);
    }

    /// <summary>
    /// Service for managing Category operations.
    /// </summary>
    /// <param name="mapper">An instance of AutoMapper that supports automatic type mapping.</param>
    /// <param name="categoryRepository">The repository for Category data access</param>
    public class CategoryService(IMapper mapper, ICategoryRepository categoryRepository) : ICategoryService
    {
        public async Task<Category> AddCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = mapper.Map<Category>(createCategoryDto);
            var result = await categoryRepository.AddAsync(category);
            return result;
        }

        public async Task UpdateCategoryAsync(EditCategoryDto editCategoryDto)
        {
            await categoryRepository.UpdateAsync(editCategoryDto.Id, category =>
            {
                mapper.Map(editCategoryDto, category);
            });
        }

        public async Task<IEnumerable<SelectListItem>> GetCategorySelectListItems()
        {
            var criteria = new RepositoryQueryOptions<CategoryFilterOptions>();
            var categoryList = await categoryRepository.GetAllAsync(criteria);
            return categoryList.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await categoryRepository.GetByIdAsync(id);
        }

        public async Task DeleteCategoryByIdAsync(int id)
        {
            await categoryRepository.DeleteByIdAsync(id);
        }
        
        public async Task<PagedResult<Category>> GetCategoriesWithForumsPagedAsync(
            int pageNumber, 
            int pageSize,
            CategoryFilterOptions categoryFilterOptions
        ) {
            var queryOptions = new PagedRepositoryQueryOptions<CategoryFilterOptions>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filter = categoryFilterOptions,
                Navigations = ["Forums"]
            };
            return await categoryRepository.GetAllPagedAsync(queryOptions);
        }
    }
}
