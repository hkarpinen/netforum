using Microsoft.AspNetCore.Identity;

namespace NETForum.Models.Entities
{
    public class User : IdentityUser<int>
    {
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserProfile? UserProfile { get; set; }
    }
}
