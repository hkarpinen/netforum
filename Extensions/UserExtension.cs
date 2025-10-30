using NETForum.Pages.Users;

namespace NETForum.Extensions;

public static class UserExtension
{
    public static UserForm ToForm(this Models.User user)
    {
        return new UserForm
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email
        };
    }
}