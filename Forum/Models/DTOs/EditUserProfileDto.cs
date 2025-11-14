namespace NETForum.Models.DTOs;

public class EditUserProfileDto
{
    public IFormFile? ProfileImage { get; set; }
    public string? Bio { get; set; }
    public string? Signature { get; set; }
    public string? Location { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}