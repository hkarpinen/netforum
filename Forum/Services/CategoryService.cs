using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using EntityFramework.Exceptions.Common;
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
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetCategorySelectListItemsAsync();
        Task<Result<Category>> AddCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<Result<EditCategoryDto>> GetCategoryForEditAsync(int id);
        Task<Result> UpdateCategoryAsync(EditCategoryDto editCategoryDto);
        Task DeleteCategoryByIdAsync(int id);
        Task<PagedResult<Category>> GetCategoriesPagedAsync(CategoryFilterOptions categoryFilterOptions);
    }
    
    public class CategoryService(IMapper mapper, AppDbContext appDbContext) : ICategoryService
    {
        public async Task<Result<Category>> AddCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            try
            {
                var category = mapper.Map<Category>(createCategoryDto);
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

        public async Task<Result> UpdateCategoryAsync(EditCategoryDto editCategoryDto)
        {
            try
            {
                var category = await appDbContext.Categories.FindAsync(editCategoryDto.Id);
                if (category == null)
                {
                    return Result.Fail("Category not found");
                }
                
                // Apply changes to entity
                mapper.Map(editCategoryDto, category);
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
            var editCategoryDto = mapper.Map<EditCategoryDto>(category);
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
        
        public async Task<PagedResult<Category>> GetCategoriesPagedAsync(CategoryFilterOptions categoryFilterOptions) {
            var categorySearchSpec = new CategorySearchSpec(categoryFilterOptions);
            
            var totalCategoryCount = await appDbContext.Categories.CountAsync();
            var categories = await appDbContext.Categories
                .WithSpecification(categorySearchSpec)
                .ToListAsync();
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = totalCategoryCount,
                Items = categories,
                PageNumber = categoryFilterOptions.PageNumber,
                PageSize = categoryFilterOptions.PageSize
            };
            return pagedResult;
        }
    }
}
