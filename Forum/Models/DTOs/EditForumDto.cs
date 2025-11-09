namespace NETForum.Models.DTOs;

public class EditForumDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Published { get; set; }
    public int? ParentForumId { get; set; }
    public int? CategoryId { get; set; }
}