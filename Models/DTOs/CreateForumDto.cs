using System.ComponentModel.DataAnnotations;

namespace NETForum.Pages.Forums;

public class CreateForumDto
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    public string? Description { get; set; }
    public bool Published { get; set; }
    public int? ParentForumId { get; set; }
    public int? CategoryId { get; set; }
}