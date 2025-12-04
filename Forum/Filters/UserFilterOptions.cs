using Microsoft.AspNetCore.Mvc;

namespace NETForum.Filters;

public enum UserSortBy
{
    Username,
    Email,
    CreatedAt
}

public class UserFilterOptions
{
    public string? Username { get; init; }
    public string? Email { get; init; }
    public UserSortBy SortBy { get; init; } = UserSortBy.CreatedAt;
    public bool Ascending { get; init; } = false;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 1;
}