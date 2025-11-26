using System.ComponentModel.DataAnnotations;

namespace NETForum.Models.DTOs;

public class CreateCategoryDto
{
    [Required] 
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool Published { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}