using NETForum.Models;
using NETForum.Pages.Account.Register;

namespace NETForum.Extensions;

public static class RegisterFormExtension
{
    public static User ToNewUser(this RegisterForm form)
    {
        return new User
        {
            UserName = form.Username,
            Email = form.Email,
            CreatedAt = DateTime.UtcNow,
            ProfileImageUrl = null
        };
    }
}