using FluentResults;

namespace NETForum.Errors;

public static class UserErrors
{
    public static Error NotFound(int id)
    {
        return new Error($"User not found with id {id}");
    }

    public static Error NotFound(string name)
    {
        return new Error($"User not found with name {name}");
    }
    
    public static Error DbUpdateFailed(int id)
    {
        return new Error($"DbUpdateFailed with id {id}");
    }

    public static Error NameTaken(string name)
    {
        return new Error($"Name {name} is already taken");
    }
}