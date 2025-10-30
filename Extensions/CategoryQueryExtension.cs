namespace NETForum.Extensions;

public static class CategoryQueryExtension
{
    public static IQueryable<Models.Category> WhereName(this IQueryable<Models.Category> query, string? name)
    {
        return !string.IsNullOrEmpty(name) 
            ? query.Where(f => f.Name.Contains(name)) 
            : query;
    }

    public static IQueryable<Models.Category> WherePublished(this IQueryable<Models.Category> query, bool? published)
    {
        return published.HasValue ? query.Where(f => f.Published.Equals(published.Value)) : query;
    }

    public static IQueryable<Models.Category> OrderByField(this IQueryable<Models.Category> query, string sortBy, bool ascending)
    {
        return sortBy?.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "created" => ascending ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Name)
        };
    }
}