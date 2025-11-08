using NETForum.Models.Entities;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyCollection<Category>> GetAllAsync(RepositoryQueryOptions<CategoryFilterOptions> options);
    Task<PagedResult<Category>> GetAllPagedAsync(PagedRepositoryQueryOptions<CategoryFilterOptions> options);
    Task<Category?> GetByIdAsync(int id, params string[] includes);
    Task<Category> AddAsync(Category category);
    Task UpdateAsync(int id, Action<Category> updateAction);
    Task DeleteByIdAsync(int id);
}