using System.ComponentModel.DataAnnotations;

namespace NETForum.Models.DTOs;

public class EditForumDto
{
    [Required] 
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public bool Published { get; set; }
    public int? ParentForumId { get; set; }
    public int? CategoryId { get; set; }
}