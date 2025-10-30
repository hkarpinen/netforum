using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NETForum.Data;
using NETForum.Extensions;
using NETForum.Models;
using NETForum.Services.Criteria;

namespace NETForum.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<SelectListItem>> GetCategorySelectListItems();
        Task<EntityEntry<Category>> AddCategoryAsync(Category category);
        Task<Category?>GetCategoryByIdAsync(int id);
        Task DeleteCategoryByIdAsync(int id);
        Task<PagedResult<Category>> GetCategoriesPagedAsync(int pageNumber, int pageSize, CategorySearchCriteria categorySearchCriteria);
    }

    public class CategoryService(AppDbContext context) : ICategoryService
    {
        public async Task<EntityEntry<Category>> AddCategoryAsync(Category category)
        {
            var result = await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<SelectListItem>> GetCategorySelectListItems()
        {
            return await context.Categories
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await context.Categories
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteCategoryByIdAsync(int id)
        {
            var category = await context.Categories
                .Where(c  => c.Id == id)
                .FirstOrDefaultAsync();
            if (category != null)
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<Category>> GetCategoriesPagedAsync(
            int pageNumber, 
            int pageSize,
            CategorySearchCriteria categorySearchCriteria
        ) {
            var query = context.Categories
                .WhereName(categorySearchCriteria.Name)
                .WherePublished(categorySearchCriteria.Published)
                .OrderByField(categorySearchCriteria.SortBy, categorySearchCriteria.Ascending);
            
            var totalCount = await query.CountAsync();
            var categories = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.Forums)
                .ToListAsync();

            return new PagedResult<Category>
            {
                Items = categories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
