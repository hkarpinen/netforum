namespace NETForum.Models;

public interface IPagedList
{
    int PageNumber { get; set; }
    int TotalPages { get; set; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}

public class PagedList<T> : List<T>, IPagedList
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    
    public PagedList() {}
    
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }
}