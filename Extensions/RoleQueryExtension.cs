namespace NETForum.Extensions;

public static class RoleQueryExtension
{
    public static IQueryable<Models.Role> WhereName(this IQueryable<Models.Role> query, string? name)
    {
        return !string.IsNullOrEmpty(name) ? query.Where(r => 
            r.Name != null &&
            r.Name.Contains(name)) : query;
    }

    public static IQueryable<Models.Role> WhereDescription(this IQueryable<Models.Role> query, string? description)
    {
        return !string.IsNullOrEmpty(description) ? query.Where(r => r.Description.Contains(description)) : query;
    }
}