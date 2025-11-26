namespace NETForum.Errors;

using FluentResults;

public static class ForumErrors
{
    public static Error NameTaken(string name)
    {
        return new Error($"The name '{name}' is already taken");
    }
    
    public static Error NotFound(int id)
    {
        return new Error($"Forum with '{id}' was not found");
    }
    
    public static Error InvalidParentForumId(int id)
    {
        return new Error($"Forum with id '{id}' cannot be it's own parent.");
    }
}