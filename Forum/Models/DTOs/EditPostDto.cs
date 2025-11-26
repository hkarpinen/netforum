using System.ComponentModel.DataAnnotations;

namespace NETForum.Models.DTOs;

public class EditPostDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    public int ForumId { get; set; }
    
    [Required]
    public int AuthorId { get; set; }
    
    [Required]
    public bool IsPinned { get; set; }
    
    [Required]
    public bool IsLocked { get; set; }
    
    [Required]
    public bool Published { get; set; }
}