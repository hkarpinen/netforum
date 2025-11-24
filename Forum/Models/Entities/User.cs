using Microsoft.AspNetCore.Identity;

namespace NETForum.Models.Entities
{
    public class User : IdentityUser<int>
    {
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public UserProfile? UserProfile { get; set; }
        public ICollection<Reply> Replies { get; set; } = new List<Reply>();
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
