namespace NETForum.Extensions
{
    public static class ForumQueryExtension
    {
        public static IQueryable<Models.Forum> WhereName(this IQueryable<Models.Forum> query, string? name)
        {
            return !string.IsNullOrEmpty(name) ? query.Where(c => c.Name.Contains(name)) : query;
        }
        
        public static IQueryable<Models.Forum> WhereCategory(this IQueryable<Models.Forum> query, int? categoryId)
        {
            return categoryId.HasValue ? query.Where(f => f.CategoryId == categoryId) : query;
        }

        public static IQueryable<Models.Forum> WherePublished(this IQueryable<Models.Forum> query, bool? published)
        {
            return published.HasValue ? query.Where(f => f.Published == published) : query;
        }

        public static IQueryable<Models.Forum> WhereParentForum(this IQueryable<Models.Forum> query, int? parentForumId)
        {
            return parentForumId.HasValue ? query.Where(f => f.ParentForumId == parentForumId) : query;
        }

        public static IQueryable<Models.Forum> OrderByField(this IQueryable<Models.Forum> query, string sortBy, bool ascending)
        {
            return sortBy?.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(f => f.Name) : query.OrderByDescending(f => f.Name),
                "created" => ascending ? query.OrderBy(f => f.CreatedAt) : query.OrderByDescending(f => f.CreatedAt),
                _ => query.OrderBy(f => f.Name)
            };
        }
    }
}
