namespace NETForum.Extensions;

public static class UserQueryExtension
{
    public static IQueryable<Models.User> WhereUsername(this IQueryable<Models.User> query, string? username)
    {
        return !string.IsNullOrEmpty(username) ? query
            .Where(u => 
                u.UserName != null && 
                u.UserName.Contains(username)
            ) : query;
    }
    
    public static IQueryable<Models.User> WhereEmail(this IQueryable<Models.User> query, string? email)
    {
        return !string.IsNullOrEmpty(email) ? query.Where(u => 
            u.Email != null &&
            u.Email.Contains(email)
        ) : query;
    }
    
    public static IQueryable<Models.User> OrderByField(this IQueryable<Models.User> query, string sortBy, bool ascending)
    {
        return sortBy?.ToLower() switch
        {
            "username" => ascending ? query.OrderBy(u => u.UserName) : query.OrderByDescending(u => u.UserName),
            "email" => ascending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            "created" => ascending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt),
            _ => query.OrderBy(u => u.UserName)
        };
    }
}