namespace NETForum.Models.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Signature { get; set; }
        public string? Location { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
