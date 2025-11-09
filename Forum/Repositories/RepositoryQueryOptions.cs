namespace NETForum.Repositories;

public class RepositoryQueryOptions<TFilter>
{
    public TFilter? Filter { get; set; }
    public string? SortBy { get; set; }
    public bool Ascending { get; set; } = true;
    public string[] Navigations { get; set; } = [];
}
