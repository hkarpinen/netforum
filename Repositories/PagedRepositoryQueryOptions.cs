namespace NETForum.Repositories;

public class PagedRepositoryQueryOptions<TFilter> : RepositoryQueryOptions<TFilter>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}