namespace NETForum.Errors;

using FluentResults;

public static class PostErrors
{
    public static Error NotFound(int id)
    {
        return new Error($"Post with '{id}' was not found");
    }
    
    public static Error AuthorNotFound(string username)
    {
        return new  Error($"Post Author with username '{username}' was not found"); 
    }

    public static Error AuthorNotFound(int id)
    {
        return new Error($"Post Author with id '{id}' was not found");
    }

    public static Error NonExistentForumId(int id)
    {
        return new Error($"Forum ID '{id}' is invalid as the forum doesn't exist");
    }
}