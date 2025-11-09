using NETForum.Models.Entities;
using NETForum.Repositories.Filters;
using NETForum.Services;

namespace NETForum.Repositories;

public interface IRoleRepository
{
    Task<IReadOnlyCollection<Role>> GetAllAsync(RepositoryQueryOptions<RoleFilterOptions> options);
    Task<Role?> GetByIdAsync(int id, params string[] includes);
    Task<PagedResult<Role>> GetAllPagedAsync(PagedRepositoryQueryOptions<RoleFilterOptions> options);
}
