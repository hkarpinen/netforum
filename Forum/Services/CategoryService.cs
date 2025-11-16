using AutoMapper;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using NETForum.Models.DTOs;
using NETForum.Models.Entities;
using NETForum.Repositories;
using NETForum.Repositories.Filters;

namespace NETForum.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetCategorySelectListItemsAsync();
        Task<Result<Category>> AddCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<Result<Category>>GetCategoryByIdAsync(int id);
        Task<Result<EditCategoryDto>> GetCategoryForEditAsync(int id);
        Task<Result> UpdateCategoryAsync(EditCategoryDto editCategoryDto);
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
        public async Task<Result<Category>> AddCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            try
            {
                var category = mapper.Map<Category>(createCategoryDto);
                var result = await categoryRepository.AddAsync(category);
                return Result<Category>.Success(result);
            }
            catch (UniqueConstraintException exception)
            {
                return Result<Category>.Failure(new Error("Category.UniqueConstraintViolation", exception.ConstraintName));
            }
        }

        public async Task<Result> UpdateCategoryAsync(EditCategoryDto editCategoryDto)
        {
            try
            {
                await categoryRepository.UpdateAsync(editCategoryDto.Id,
                    category => { mapper.Map(editCategoryDto, category); });
                return Result.Success();
            }
            catch (UniqueConstraintException exception)
            {
                return Result.Failure(new Error("Category.UniqueConstraintViolation", exception.ConstraintName));
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetCategorySelectListItemsAsync()
        {
            var criteria = new RepositoryQueryOptions<CategoryFilterOptions>();
            var categoryList = await categoryRepository.GetAllAsync(criteria);
            return categoryList.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }

        public async Task<Result<Category>> GetCategoryByIdAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            return category == null ?
                Result<Category>.Failure(new Error("Category.NotFound", $"Category with ID: {id} was not found.")) : 
                Result<Category>.Success(category);
        }

        public async Task<Result<EditCategoryDto>> GetCategoryForEditAsync(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return Result<EditCategoryDto>.Failure(new Error("Category.NotFound", $"Category with ID: {id} was not found."));
            }
            var editCategoryDto = mapper.Map<EditCategoryDto>(category);
            return Result<EditCategoryDto>.Success(editCategoryDto);
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
