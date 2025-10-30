using NETForum.Pages.Users;

namespace NETForum.Extensions;

public static class UserFormExtension
{
    public static Models.User ToNewUser(this UserForm userForm)
    {
        return new Models.User
        {
            UserName = userForm.Username,
            Email = userForm.Email,
            EmailConfirmed = false
        };
    }
}