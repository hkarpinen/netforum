using Microsoft.AspNetCore.Identity;

namespace NETForum.Models
{
    public class Role : IdentityRole<int>
    {
        public required string Description { get; set; }
    }
}
