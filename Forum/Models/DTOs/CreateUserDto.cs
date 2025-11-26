using System.ComponentModel.DataAnnotations;

namespace NETForum.Models.DTOs;

public class CreateUserDto
{
    [Required]
    public string Username { get; set; } =  string.Empty;
    
    [Required]
    public string Email { get; set; } = string.Empty;
}