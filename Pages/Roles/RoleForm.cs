using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Roles
{
    public class RoleForm
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;
    }
}
