using Ardalis.Specification.EntityFrameworkCore;
using EntityFramework.Exceptions.Common;
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
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetCategorySelectListItemsAsync();
        Task<Result<Category>> AddCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<Result<EditCategoryDto>> GetCategoryForEditAsync(int id);
        Task<Result> UpdateCategoryAsync(int id, EditCategoryDto editCategoryDto);
        Task DeleteCategoryByIdAsync(int id);
        Task<PagedList<Category>> GetCategoriesPagedAsync(CategoryFilterOptions categoryFilterOptions);
    }
    
    public class CategoryService(AppDbContext appDbContext) : ICategoryService
    {
        public async Task<Result<Category>> AddCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            try
            {
                // Map Create DTO to Category
                var category = new Category
                {
                    Name = createCategoryDto.Name,
                    Description = createCategoryDto.Description,
                    SortOrder = createCategoryDto.SortOrder,
                    CreatedAt = DateTime.UtcNow,
                    Published = createCategoryDto.Published,
                    UpdatedAt = DateTime.UtcNow
                };
                
                var result = await appDbContext.Categories.AddAsync(category);
                await appDbContext.SaveChangesAsync();
                return Result.Ok(result.Entity);
            }
            catch (UniqueConstraintException exception)
            {
                var constraintShortName = exception.ConstraintName.Split("_").Last();
                return Result.Fail<Category>($"Category {constraintShortName} is already in use.");
            }
        }

        public async Task<Result> UpdateCategoryAsync(int id, EditCategoryDto editCategoryDto)
        {
            try
            {
                var category = await appDbContext.Categories.FindAsync(id);
                if (category == null)
                {
                    return Result.Fail("Category not found");
                }
                
                // Map Edit DTO to Category
                category.Name = editCategoryDto.Name;
                category.Description = editCategoryDto.Description;
                category.SortOrder = editCategoryDto.SortOrder;
                category.Published = editCategoryDto.Published;
                category.UpdatedAt = DateTime.UtcNow;
                
                await appDbContext.SaveChangesAsync();
                return Result.Ok();
            }
            catch (UniqueConstraintException exception)
            {
                var constraintShortName = exception.ConstraintName.Split("_").Last();
                return Result.Fail($"Category {constraintShortName} is already in use.");
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetCategorySelectListItemsAsync()
        {
            return await appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToListAsync();
        }
        
        public async Task<Result<EditCategoryDto>> GetCategoryForEditAsync(int id)
        {
            var category = await appDbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return Result.Fail<EditCategoryDto>("Category not found");
            }
            
            // Map Category to Edit DTO
            var editCategoryDto = new EditCategoryDto
            {
                Name = category.Name,
                Description = category.Description,
                SortOrder = category.SortOrder,
                Published = category.Published,
                UpdatedAt = category.UpdatedAt,
                CreatedAt = category.CreatedAt
            };
            
            return  Result.Ok(editCategoryDto);
        }

        public async Task DeleteCategoryByIdAsync(int id)
        {
            var entity = await appDbContext.Categories.FindAsync(id);
            if (entity != null)
            {
                appDbContext.Categories.Remove(entity);
                await appDbContext.SaveChangesAsync();
            }
        }
        
        public async Task<PagedList<Category>> GetCategoriesPagedAsync(CategoryFilterOptions categoryFilterOptions) {
            var categorySearchSpec = new CategorySearchSpec(categoryFilterOptions);
            
            var totalCategoryCount = await appDbContext.Categories.CountAsync();
            var categories = await appDbContext.Categories
                .WithSpecification(categorySearchSpec)
                .ToListAsync();
            var pagedList = new PagedList<Category>(
                categories,
                totalCategoryCount,
                categoryFilterOptions.PageNumber,
                categoryFilterOptions.PageSize
            );
            return pagedList;
        }
    }
}
