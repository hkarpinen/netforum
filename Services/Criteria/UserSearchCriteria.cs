namespace NETForum.Services.Criteria;

public class UserSearchCriteria
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string SortBy { get; set; }
    public bool Ascending { get; set; } = true;
}