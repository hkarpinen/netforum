namespace NETForum.Models.DTOs
{
    public class CreateUserProfileDto
    {
        public int UserId { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? Bio { get; set; }
        public string? Signature { get; set; }
        public string? Location { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }
}
