namespace NETForum.Services
{
    public class PagedResult<TEntity>
    {
        public IReadOnlyCollection<TEntity> Items { get; set; } = new List<TEntity>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int) Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

    }
}
