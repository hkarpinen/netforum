using System.ComponentModel.DataAnnotations;

namespace NETForum.Models.DTOs;

public class EditCategoryDto
{
    //public int Id { get; set; }
    [Required] 
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    
    [Required]
    public bool Published { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}