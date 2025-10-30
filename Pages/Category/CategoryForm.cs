using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Category
{
    public class CategoryForm
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int SortOrder { get; set; }
        
        [Required]
        public bool Published { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
