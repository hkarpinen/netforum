using NETForum.Models;
using NETForum.Pages.Account.Profile;

namespace NETForum.Extensions;

public static class UserProfileFormExtension
{
    public static UserProfile ToUserProfile(this UserProfileForm userProfileForm)
    {
        return new UserProfile
        {
            Bio = userProfileForm.Bio,
            Signature = userProfileForm.Signature,
            Location = userProfileForm.Location,
            DateOfBirth = userProfileForm.DateOfBirth,
            LastUpdated = DateTime.UtcNow
        };
    }
}