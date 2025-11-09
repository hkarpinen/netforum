using Microsoft.EntityFrameworkCore;
using NETForum.Data;
using NETForum.Services;

namespace NETForum.Repositories;

/// <summary>
/// Base repository providing common data access operations for Entity Framework entities.
/// </summary>
/// <typeparam name="TEntity">The entity type that this repository manages.</typeparam>
/// <typeparam name="TFilter">The available options to filter the query by.</typeparam>
public abstract class BaseRepository<
    TEntity, 
    TFilter
> where TEntity : class {
    protected readonly AppDbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseRepository(AppDbContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<TEntity>();
    }
    
    public abstract Task<TEntity?> GetByIdAsync(int id, params string[] includes);
    
    /// <summary>
    /// Applies a filter to an existing query.
    /// </summary>
    /// <param name="query">The base query to filter.</param>
    /// <param name="filter">The filter to apply.</param>
    /// <returns>The filtered query.</returns>
    protected abstract IQueryable<TEntity> ApplyFilter(IQueryable<TEntity> query, TFilter filter);
    
    /// <summary>
    /// Applies navigation property include to the query for eager loading of related data.
    /// </summary>
    /// <param name="query">The base query to apply includes to.</param>
    /// <param name="navigationProperties">Navigation property paths to include.</param>
    /// <returns>The query with included navigation properties.</returns>
    protected abstract IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, params string[] navigationProperties);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="sortBy"></param>
    /// <param name="ascending"></param>
    /// <returns></returns>
    protected abstract IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, string sortBy, bool ascending);

    /// <summary>
    /// Utility method to conditionally apply filters, includes (table joins) and sorting options.
    /// </summary>
    /// <param name="query">A query object of type <typeparamref name="TEntity"/></param>
    /// <param name="repositoryQueryOptions"></param>
    /// <returns>A query with filters, includes and sorting options</returns>
    protected IQueryable<TEntity> ApplyQueryOptions(IQueryable<TEntity> query, RepositoryQueryOptions<TFilter> repositoryQueryOptions)
    {
        // Apply relationships joins if present
        if (repositoryQueryOptions.Navigations.Length > 0)
        {
            query = ApplyIncludes(query, repositoryQueryOptions.Navigations);
        }
        
        // Apply criteria if present
        if (repositoryQueryOptions.Filter != null)
        {
            query = ApplyFilter(query, repositoryQueryOptions.Filter);
        }
        
        // Apply sorting if present
        if (repositoryQueryOptions.SortBy != null)
        {
            query = ApplySorting(query, repositoryQueryOptions.SortBy, repositoryQueryOptions.Ascending);
        }

        return query;
    }

    /// <summary>
    /// Gets all entities of the specified type with optional filtering, sorting, and included navigation properties.
    /// </summary>
    /// <param name="repositoryQueryOptions">Query configuration including filters, sort order, and navigation properties to include.</param>
    /// <returns>A read only collection of matching entities of the specified type.</returns>
    public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(RepositoryQueryOptions<TFilter> repositoryQueryOptions)
    {
        var query = _dbSet.AsQueryable();
        query = ApplyQueryOptions(query, repositoryQueryOptions);
        return await query.ToListAsync();
    }

    /// <summary>
    /// Gets paginated entities of the specified type with optional filtering, sorting, included navigation properties.
    /// </summary>
    /// <param name="repositoryQueryOptions">An object containing criteria, includes, sorting, and pagination options</param>
    /// <returns>A PagedResult of matching entities, current page number/size, and total count of matches.</returns>
    public virtual async Task<PagedResult<TEntity>> GetAllPagedAsync(PagedRepositoryQueryOptions<TFilter> repositoryQueryOptions)
    {
        var query = _dbSet.AsQueryable();
        query = ApplyQueryOptions(query, repositoryQueryOptions);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((repositoryQueryOptions.PageNumber - 1) * repositoryQueryOptions.PageSize)
            .Take(repositoryQueryOptions.PageSize)
            .ToListAsync();
        return new PagedResult<TEntity>
        {
            TotalCount = totalCount,
            Items = items,
            PageNumber = repositoryQueryOptions.PageNumber,
            PageSize = repositoryQueryOptions.PageSize,
        };
    }

    /// <summary>
    /// Adds a new entity of type <typeparamref name="TEntity"/> to the database
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        var result = await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    /// <summary>
    /// Updating an existing entity by applying changes to the tracked Entity Framework Core entity.
    /// </summary>
    /// <param name="id">The primary key of the entity to update</param>
    /// <param name="updateAction">An action that modifies the tracked entity's properties</param>
    /// <exception cref="KeyNotFoundException">Thrown when no entity with the specified ID exists.</exception>
    public virtual async Task UpdateAsync(int id, Action<TEntity> updateAction)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) throw new KeyNotFoundException($"{_dbSet.EntityType.Name} with id {id} not found");
        
        // Invoking service applies any changes to the tracked entity.
        updateAction(entity);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Idempotent entity deletion of type <typeparamref name="TEntity"/>
    /// </summary>
    /// <param name="id">The primary key of the entity to delete</param>
    public virtual async Task DeleteByIdAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}