using System.ComponentModel.DataAnnotations;

namespace NETForum.Models.DTOs;

public class EditRoleDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required] public string Description { get; set; } = string.Empty;
}