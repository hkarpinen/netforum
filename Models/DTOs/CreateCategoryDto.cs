namespace NETForum.Models.DTOs;

public class CreateCategoryDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool Published { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}